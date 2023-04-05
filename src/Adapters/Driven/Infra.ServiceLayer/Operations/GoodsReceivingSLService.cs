using System.Net;
using System.Text.Json;
using Domain.Entities;
using Domain.Entities.ReceiptOfGoods;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace Infra.ServiceLayer.Operations;

public class GoodsReceivingSLService : IGoodsReceivingSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<GoodsReceivingSLService> _logger;

    public GoodsReceivingSLService(IConfiguration configuration, IHttpClientFactory httpClientFactory, AsyncCircuitBreakerPolicy circuitBreaker, ILoginSLService loginService, ILogger<GoodsReceivingSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task CreateDeliveryNote(DeliveryNotesCreated deliveryNotesCreated, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var x = JsonSerializer.Serialize(deliveryNotesCreated);

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync($"/b1s/v1/PurchaseDeliveryNotes", new StringContent(JsonSerializer.Serialize(deliveryNotesCreated)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await CreateDeliveryNote(deliveryNotesCreated, 1);
        }

        if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetSerialNumbersObjBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task UpdateSerialNumbersObjBySerieAsync(Preparation preparation, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
   
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/SerialNumberDetails({docEntry})", new StringContent(JsonSerializer.Serialize(preparation)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateSerialNumbersObjBySerieAsync(preparation, docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateSerialNumbersObjBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }


    public async Task<GoodsReceiving?> GetPurchaseOrderByKeyAcessAsync(string keyaccess, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=5000");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('sql_purchase_order_by_key_access')/List?keyacces='{keyaccess}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetPurchaseOrderByKeyAcessAsync(keyaccess, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetPurchaseOrderByKeyAcessAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<GoodsReceiving>(json);
    }

    public async Task<ReceivingItem?> GetQuantityReceivingPurchaseOrderByDocEntryAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=5000");

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('PurchaseOrderItensReceiving')/List?docEntry='{docEntry}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetQuantityReceivingPurchaseOrderByDocEntryAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetQuantityReceivingPurchaseOrderByDocEntryAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<ReceivingItem>(json);
    }

    public async Task<ScheduleReceiving?> GetSchedulingPurchaseOrderByDateAsync(DateTime startAt, DateTime finishAt, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=80");
        var startDate = (startAt != default ? startAt : DateTime.Now).ToString("yyyy-MM-dd");
        var finishDate = (finishAt != default ? finishAt : DateTime.Now).ToString("yyyy-MM-dd");

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/PurchaseOrders?$filter=(DocDueDate ge '{startDate}' and DocDueDate le '{finishDate}')&$select=CardName,Comments,DocDueDate,DocNum,DocEntry,NumAtCard,U_WMS_Status");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetSchedulingPurchaseOrderByDateAsync(startAt, finishAt, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetSchedulingPurchaseOrderByDateAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<ScheduleReceiving>(json);
    }

    public async Task<SerialNumbersObj?> GetSerialNumbersObjBySerieAsync(string serial, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SerialNumberDetails?$filter=SerialNumber eq '{serial}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetSerialNumbersObjBySerieAsync(serial, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetSerialNumbersObjBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<SerialNumbersObj>(json);
    }

    public async Task<SeriesItem?> GetSeriesItemAsync(long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=3000");

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SeriesItems('{docEntry}')");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetSeriesItemAsync(docEntry, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<SeriesItem>(json);
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

    public async Task PostSeriesItemAsync(SeriesItem seriesItem, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync($"/b1s/v1/SeriesItems", new StringContent(JsonSerializer.Serialize(seriesItem)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await PostSeriesItemAsync(seriesItem, 1);
            return;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Created)
            return;

        if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"PostSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return;
    }

    public async Task UpdateHeaderPurchaseOrder(PurchaseOrderUpdateHeader purchaseOrder, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/PurchaseOrders({docEntry})", new StringContent(JsonSerializer.Serialize(purchaseOrder)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateHeaderPurchaseOrder(purchaseOrder, docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"GetPurchaseOrderByKeyAcessAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }


    public async Task UpdateItemByPurchaseOrder(PurchaseOrderUpdate purchaseOrder, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/PurchaseOrders({docEntry})", new StringContent(JsonSerializer.Serialize(purchaseOrder)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateItemByPurchaseOrder(purchaseOrder, docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"GetPurchaseOrderByKeyAcessAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return;
    }

    public async Task UpdateSerialPatrimonyBySerial(UpdateSerialNumber updateSerialNumber, string docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/SerialNumberDetails({docEntry})", new StringContent(JsonSerializer.Serialize(updateSerialNumber)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateSerialPatrimonyBySerial(updateSerialNumber, docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.Unauthorized)
            throw new Exception($"UpdateSerialPatrimonyBySerial - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return;

    }

    public async Task UpdateSeriesItemHeaderAsync(SeriesItemHeader seriesItemHeader, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/SeriesItems('{docEntry}')", new StringContent(JsonSerializer.Serialize(seriesItemHeader)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateSeriesItemHeaderAsync(seriesItemHeader, docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task UpdateSeriesItemLineAsync(SeriesItem seriesItem, long docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");

        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/SeriesItems('{docEntry}')", new StringContent(JsonSerializer.Serialize(seriesItem)));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateSeriesItemLineAsync(seriesItem, docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateSeriesItemAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }
}