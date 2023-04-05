using System.Net;
using System.Text.Json;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace Infra.ServiceLayer.Operations;

public class BranchesSLService : IBranchesSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<BranchesSLService> _logger;

    public BranchesSLService(IConfiguration configuration,
                             IHttpClientFactory httpClientFactory,
                             AsyncCircuitBreakerPolicy circuitBreaker,
                             ILoginSLService loginService,
                             ILogger<BranchesSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<Branches> GetAllBranch(int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('allBranchData')/List");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetAllBranch( 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetAllBranch - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Branches>(json);
    }

    public async Task<Branches> GetBranch(int bplId, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('branchData')/List?bplId={bplId}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetBranch(bplId, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetBranch - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Branches>(json);
    }
}