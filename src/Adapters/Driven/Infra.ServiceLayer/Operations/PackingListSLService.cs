using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class PackingListSLService : IPackingListSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<PackingListSLService> _logger;

    public PackingListSLService(IConfiguration configuration,
                                IHttpClientFactory httpClientFactory,
                                AsyncCircuitBreakerPolicy circuitBreaker,
                                ILoginSLService loginService,
                                ILogger<PackingListSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<PackingList> CreatePackingListAsync(string carrierId, string method, int branch, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PostAsync($"/b1s/v1/PackingList",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_CarrierId = carrierId,
                            U_Method = method,
                            U_BPLId = branch
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await CreatePackingListAsync(carrierId, method, branch, 1);
        }

        if (response.StatusCode != HttpStatusCode.Created)
            throw new Exception($"CreatePackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<PackingList>(json) ?? throw new ArgumentNullException("json is null");
    }


    public async Task<PackingList?> GetPackingListAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/PackingList({docEntry})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetPackingListAsync(docEntry, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<PackingList>(json);
    }

    public async Task RemovePackingListAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.DeleteAsync($"/b1s/v1/PackingList({docEntry})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await RemovePackingListAsync(docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent || response.StatusCode != HttpStatusCode.NotFound)
            throw new Exception($"RemovePackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<string?> ClosePackingListAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PostAsync($"/b1s/v1/PackingList({docEntry})/Close", new StringContent("", Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await ClosePackingListAsync(docEntry, 1);
            return "ok";
        }

        if (response.StatusCode != HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        return "ok";
    }

    public async Task<string?> UpdateDateClosePackingListAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/PackingList({docEntry})",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_DateClosedAt = DateTime.Now.ToString("yyyy-MM-dd"),
                            U_TimeClosedAt = DateTime.Now.ToString("HH:mm:ss"),
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await ClosePackingListAsync(docEntry, 1);
            return "ok";
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        return "ok";
    }

    public async Task<(string NumAtCard, long DocEntry, long DocNum, string CardName, long SequenceSerial, string SeriesStr)> GetInvoiceEntryAsync(string keyNfe, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Invoices/?$filter=U_ChaveAcesso eq '{keyNfe}'&$select=NumAtCard,DocEntry,CardName,DocNum,SequenceSerial,SeriesString");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetInvoiceEntryAsync(keyNfe, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetInvoiceEntryAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("response login service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        
        var docEntry = result["value"]?[0]?["DocEntry"];
        var numAtCard = result["value"]?[0]?["NumAtCard"];
        var docNum = result["value"]?[0]?["DocNum"];
        var cardName = result["value"]?[0]?["CardName"];
        var sequenceSerial = result["value"]?[0]?["SequenceSerial"];
        var seriesStr = result["value"]?[0]?["SeriesString"];

        if (docEntry == null)
            return default;
        
        return new((string)numAtCard, (int)docEntry, (int)docNum, cardName.ToString(),(int)sequenceSerial, seriesStr.ToString());
    }

    public async Task AddItemPackingList(long packingListEntry, PackingListUpsert itemPackList, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/PackingList({packingListEntry})",
                        new StringContent(JsonSerializer.Serialize(itemPackList), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await AddItemPackingList(packingListEntry, itemPackList, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"AddItemPackingList - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task RemoveItemPackingListAsync(long packingListEntry, PackingList packingList, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PutAsync($"/b1s/v1/PackingList({packingListEntry})",
                        new StringContent(JsonSerializer.Serialize(packingList), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await RemoveItemPackingListAsync(packingListEntry, packingList, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"RemoveItemPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<PackingListRoot?> GetAllPackingListAsync(DateTime startAt, DateTime finishAt, string status, string bplId, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=120");

        var startDate = (startAt != default ? startAt : DateTime.Now).ToString("yyyy-MM-dd");
        var finishDate = (finishAt != default ? finishAt : DateTime.Now).ToString("yyyy-MM-dd");
        var statusSearch = status == "T" ? "" : $"and Status eq '{status}'"; 

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/PackingList?$filter=(CreateDate ge '{startDate}' and CreateDate le '{finishDate}' and U_BPLId eq {bplId} {statusSearch})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetAllPackingListAsync(startAt, finishAt, status, bplId, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetAllPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

        return JsonSerializer.Deserialize<PackingListRoot>(json);
    }

    public async Task<InfoNFPeerValidation?> GetInfoNFEAsync(string keyNfe, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/SQLQueries('order-peer-key')/List?keyAccess='{keyNfe}'&doc={keyNfe}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetInfoNFEAsync(keyNfe, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetInfoNFEAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("N�o encontrado dados para nota.");

        return JsonSerializer.Deserialize<InfoNFPeerValidation>(json);
    }

    public async Task UpdateOrderByPackingList(long docEntry, string packingListId, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var status = "Packing";
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status, U_CT_PackingListId = packingListId }),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateOrderByPackingList(docEntry, packingListId, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateOrderByPackingList - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task UpdateOrderDispached(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var status = "Shipped";
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status }),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateOrderDispached(docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateOrderDispached - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task UpdateDateDispatchPackingListAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/PackingList({docEntry})",
                        new StringContent(JsonSerializer.Serialize(new
                        {
                            U_DateExportedAt = DateTime.Now.ToString("yyyy-MM-dd")
                        }), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateDateDispatchPackingListAsync(docEntry, 1);
            return;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return;

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateDateDispatchPackingListAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

    }

    public async Task<(string AccessKey, string NumAtCard, long DocEntry, long DocNum, string CardName, long SequenceSerial, string SeriesStr)> GetInvoiceEntryBySerialAsync(string keyNfe, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Invoices/?$filter=SequenceSerial  eq {keyNfe}&$select=U_ChaveAcesso,NumAtCard,DocEntry,CardName,DocNum,SequenceSerial,SeriesString");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetInvoiceEntryBySerialAsync(keyNfe, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetInvoiceEntryAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("response login service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");

        var keyAcess = result["value"]?[0]?["U_ChaveAcesso"];
        var docEntry = result["value"]?[0]?["DocEntry"];
        var numAtCard = result["value"]?[0]?["NumAtCard"];
        var docNum = result["value"]?[0]?["DocNum"];
        var cardName = result["value"]?[0]?["CardName"];
        var sequenceSerial = result["value"]?[0]?["SequenceSerial"];
        var seriesStr = result["value"]?[0]?["SeriesString"];

        if (docEntry == null)
            return default;

        return new((string)keyAcess, (string)numAtCard, (int)docEntry, (int)docNum, cardName.ToString(), (int)sequenceSerial, seriesStr.ToString());
    }
}