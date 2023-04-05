using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class LoginSLService : ILoginSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILogger<LoginSLService> _logger;
    public string _sessionId = "";

    public LoginSLService(IConfiguration configuration,
                        IHttpClientFactory httpClientFactory,
                        AsyncCircuitBreakerPolicy circuitBreaker,
                        ILogger<LoginSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _logger = logger;
    }

    public async Task<string> TokenAsync()
    {
        if (string.IsNullOrWhiteSpace(_sessionId))
            await LoginAsync();

        return _sessionId;
    }

    public async Task LoginAsync()
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync("/b1s/v1/Login",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            CompanyDB = _configuration.GetSection("Api:ServiceLayer:CompanyDB").Value,
                            Password = _configuration.GetSection("Api:ServiceLayer:Password").Value,
                            UserName = _configuration.GetSection("Api:ServiceLayer:UserName").Value,
                            Language = Convert.ToInt32(_configuration.GetSection("Api:ServiceLayer:Language").Value)
                        }), Encoding.UTF8, Application.Json));
        });

        if (!response.IsSuccessStatusCode)
            throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("response login service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        var sessionId = result["SessionId"];

        if (sessionId == null)
            throw new Exception("sessionId is null");

        _sessionId = sessionId.ToString();
    }
}

