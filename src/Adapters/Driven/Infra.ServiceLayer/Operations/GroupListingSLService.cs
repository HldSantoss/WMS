using System.Net;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class GroupListingSLService : IGroupListingSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<GroupListingSLService> _logger;

    public GroupListingSLService(IConfiguration configuration,
                                 IHttpClientFactory httpClientFactory,
                                 AsyncCircuitBreakerPolicy circuitBreaker,
                                 ILoginSLService loginService,
                                 ILogger<GroupListingSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<IEnumerable<GroupListing>> GetGroupListing(int? bplId = null, int tryLogin = 0)
    {
        var queryString = "";

        if (bplId != null)
        {
            queryString = $"&$filter = U_CT_Branch eq {bplId}";
        }

        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/PICKINGGROUP/?$select=Code,Name,DocEntry,CreateDate,UpdateDate,CreateTime,UpdateTime{queryString}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetGroupListing(bplId, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetGroupListing - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        var lists = JsonSerializer.Deserialize<GroupListingList>(json) ?? throw new ArgumentNullException("body service layer");

        return lists.GroupListing;
    }

    public async Task<GroupListing> CreateGroupListing(string code, string name, int? bplId = null, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync("/b1s/v1/PICKINGGROUP",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            Code = code,
                            Name = name,
                            U_CT_Branch = bplId
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await CreateGroupListing(code, name, bplId, 1);
        }

        if (response.StatusCode != HttpStatusCode.Created)
            throw new Exception($"status={response.StatusCode} - payload={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<GroupListing>(json) ?? throw new ArgumentNullException("body service layer");
    }

    public async Task<GroupListing?> GetGroupListing(string code, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/PICKINGGROUP('{code}')");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetGroupListing(code, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetGroupListing - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<GroupListing>(json) ?? throw new ArgumentNullException("body service layer");
    }

    public async Task<string?> DeleteGroupListing(string code, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.DeleteAsync($"/b1s/v1/PICKINGGROUP('{code}')");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await DeleteGroupListing(code, 1);
            return "";
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetGroupListing - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        return "";
    }

    public async Task<string?> AddUserGroupListingAsync(string code, IEnumerable<User> usersCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/PICKINGGROUP('{code}')",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            PICKINGGROUPUSERSCollection = usersCode
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await AddUserGroupListingAsync(code, usersCode, 1);
            return "";
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetGroupListing - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        return "";
    }

    public async Task DeleteUserGroupListingAsync(string code, string bplId, List<User> usersStay, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PutAsync($"/b1s/v1/PICKINGGROUP('{code}')",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_CT_Branch = bplId,
                            PICKINGGROUPUSERSCollection = usersStay
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await DeleteUserGroupListingAsync(code, bplId, usersStay, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }
}
