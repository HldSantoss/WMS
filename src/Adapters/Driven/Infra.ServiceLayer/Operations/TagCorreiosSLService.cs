using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class TagCorreiosSLService : ITagCorreiosSLService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<TagCorreiosSLService> _logger;

    public TagCorreiosSLService(IHttpClientFactory httpClientFactory,
                                AsyncCircuitBreakerPolicy circuitBreaker,
                                ILoginSLService loginService,
                                ILogger<TagCorreiosSLService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<SroData?> GetCurrent(string method, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/WmCorreios/?$filter=U_Method eq '{method}' and U_IsFinish eq 'N'&$select=Code,U_End,U_Current,U_Prefix,U_Suffix,U_Contract");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetCurrent(method, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("Pedido inexistente");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        var current = result["value"]?[0];
        
        if (current == null)
            return null;

        return JsonSerializer.Deserialize<SroData>(current);
    }

    public async Task SetCurrent(string Code, int Current, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/WmCorreios('{Code}')",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_Current = Current
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"status={response.StatusCode} - body={response?.Content?.ReadAsStringAsync()?.Result}");
    }

    public async Task SetFinished(string Code, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/WmCorreios('{Code}')",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_IsFinish = "N"
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"status={response.StatusCode} - body={response?.Content?.ReadAsStringAsync()?.Result}");
    }
}