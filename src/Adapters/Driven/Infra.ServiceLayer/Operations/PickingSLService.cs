using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.DTO;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class PickingSLService : IPickingSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<PickingSLService> _logger;

    public PickingSLService(IConfiguration configuration,
                            IHttpClientFactory httpClientFactory,
                            AsyncCircuitBreakerPolicy circuitBreaker,
                            ILoginSLService loginService,
                            ILogger<PickingSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<Picking?> GetPickingAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Orders({docEntry})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetPickingAsync(docEntry, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("Pedido inexistente");
        return JsonSerializer.Deserialize<Picking>(json) ?? throw new ArgumentNullException("Pedido inexistente");
    }

    public async Task<PickingUser?> GetPickingLoginAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Orders({docEntry})?$select=U_CT_LoginWms,U_WMS_Status");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetPickingLoginAsync(docEntry, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("Pedido inexistente");
        return JsonSerializer.Deserialize<PickingUser>(json) ?? throw new ArgumentNullException("Pedido inexistente");
    }

    public async Task<PickingGroup?> GetUsersPickingGroupAsync(int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/PICKINGGROUP/?$select=Code,PICKINGGROUPUSERSCollection");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetUsersPickingGroupAsync(1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetUsersPickingGroup - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<PickingGroup>(json);
    }

    public async Task<long?> GetNextPickingAsync(string groupId, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Orders?$filter=(DocumentStatus eq 'O' and contains(U_PickingGroup, {groupId}) and U_WMS_Status eq 'CanPick')&$select=DocEntry&$orderby=TaxDate&$top=1");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetNextPickingAsync(groupId, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetNextPickingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var nextPick = JsonSerializer.Deserialize<DocEntryByQuery>(json);

        if (nextPick == null || !nextPick.DocEntries.Any())
            return null;

        return nextPick.DocEntries.First().DocEntry;
    }

    public async Task UpdatePickingStatusAsync(string status, long docEntry, string user, string? sro = null, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status, U_CT_TrackingCode = sro, U_CT_LoginWms = user}),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdatePickingStatusAsync(status, docEntry, user, sro, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetNextPickingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<BinsPicking?> GetBinsPickingAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/SQLQueries('BinsForPicking')/List?docEntry={docEntry}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetBinsPickingAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetNextPickingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<BinsPicking>(json);
    }

    public async Task<long> GetDocEntryPickSavedAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Picking/?$filter=U_OrderId eq {docEntry}&$select=DocEntry");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetDocEntryPickSavedAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            return 0;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetDocEntryPickSavedAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var nextPick = JsonSerializer.Deserialize<DocEntryByQuery>(json);

        if (nextPick == null || !nextPick.DocEntries.Any())
            return 0;

        return nextPick.DocEntries.First().DocEntry;
    }

    public async Task RemovePickSavedAsync(long pickEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.DeleteAsync($"/b1s/v1/Picking({pickEntry})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await RemovePickSavedAsync(pickEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NotFound)
            _logger.LogDebug($"removePicking status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetDocEntryPickSavedAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task CreatePickSavedAsync(Picking picking, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync($"/b1s/v1/Picking", new StringContent(JsonSerializer.Serialize(new { U_OrderId = picking.DocEntry, U_UserId = "", U_DocDueDate = DateTime.Now, U_Payload = JsonSerializer.Serialize(picking) }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await CreatePickSavedAsync(picking, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetDocEntryPickSavedAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<BinsPicking?> SuggestNextBinPickingAsync(string itemCode, string binCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/SQLQueries('suggestBinsForPicking')/List?itemCode='{itemCode}'&binCode='{binCode}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await SuggestNextBinPickingAsync(itemCode, binCode, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"SuggestNextBinPickingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<BinsPicking>(json);
    }

    public async Task<SavedPickingDto?> GetSavedPickingAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Picking/?$filter=U_OrderId eq {docEntry}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetSavedPickingAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetSavedPickingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<SavedPickingDto>(json);
    }

    public async Task<bool> KeepGoingAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Orders/?$filter=U_WMS_Status eq 'SavePicking' and DocEntry eq {docEntry}&$select=DocEntry");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await KeepGoingAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"KeepGoingAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var nextPick = JsonSerializer.Deserialize<DocEntryByQuery>(json);

        if (nextPick == null || !nextPick.DocEntries.Any())
            return false;

        return nextPick.DocEntries.First().DocEntry > 0;
    }

    public async Task<string> GetCarrierName(string carrier)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/BusinessPartners('{carrier}')/CardName");
        });

        if (!response.IsSuccessStatusCode)
            throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("response login service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        var cardName = result["value"]  ?? throw new ArgumentNullException($"Transportadora {carrier} não possui nome cadastrado");

        return cardName.ToString();
    }

    public async Task UpdatePickingStatusByCheckoutAsync(string status, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status }),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdatePickingStatusByCheckoutAsync(status, docEntry, 1);
            return;
        }

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

    }

    public async Task UpdatePickingStatusReplenishAsync(string status, long docEntry, string itemCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status, Comments = itemCode }),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdatePickingStatusReplenishAsync(status, docEntry, itemCode, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdatePickingStatusReplenishAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<ErrorIntegration?> GetErrorDetails(int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/ERROR?$select=CreateDate,Creator,Remark,U_Details,U_OrderId");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetErrorDetails(1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetErrorDetails - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<ErrorIntegration>(json);
    }
}