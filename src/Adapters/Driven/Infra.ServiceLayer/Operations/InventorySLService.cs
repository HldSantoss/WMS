using System.Net;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Entities.Inventories;
using Domain.ValueObject;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations;

public class InventorySLService : IInventorySLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<InventorySLService> _logger;

    public InventorySLService(IConfiguration configuration,
                            IHttpClientFactory httpClientFactory,
                            AsyncCircuitBreakerPolicy circuitBreaker,
                            ILoginSLService loginService,
                            ILogger<InventorySLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<Inventory?> InventoryBalanceBinLocationsByItemCodeAsync(string sku, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=600");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('stockBinLocationBySku')/List?itemCode='{sku}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await InventoryBalanceBinLocationsByItemCodeAsync(sku, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"StockBinLocationsBySku - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Inventory>(json);
    }

    public async Task<Inventory?> InventoryBalanceBinLocationsBySerieAsync(string serie, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=600");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('stockBinLocationBySerie')/List?serie='{serie}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await InventoryBalanceBinLocationsBySerieAsync(serie, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"InventoryBalanceBinLocationsBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Inventory>(json);
    }

    public async Task<Inventory?> InventoryBalanceBinLocationsByBinCodeAsync(string binCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=600");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('stockBinLocationByBinCode')/List?bincode='{binCode}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await InventoryBalanceBinLocationsByBinCodeAsync(binCode, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"InventoryBalanceBinLocationsByBinCodeAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Inventory>(json);
    }

    public async Task<(int binAbsFrom, int binAbsTo, string whsCodeFrom, string whsCodeTo)> BinAbsByBinCodeAsync(string binCodeFrom, string binCodeTo, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('BinAbsFromBinCode')/List?binCodeFrom='{binCodeFrom}'&binCodeTo='{binCodeTo}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await BinAbsByBinCodeAsync(binCodeFrom, binCodeTo, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"BinAbsFromBinCode - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"BinAbsFromBinCode status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var bins = JsonSerializer.Deserialize<BinLocation>(json) ?? throw new ArgumentNullException("body service layer BinAbsFromBinCode");

        if (bins.Items.Count < 2)
            return default;

        return new((int)bins.Items.First(p => p.BinCode == binCodeFrom).AbsEntry,
                    (int)bins.Items.First(p => p.BinCode == binCodeTo).AbsEntry,
                    bins.Items.First(p => p.BinCode == binCodeFrom).WhsCode,
                    bins.Items.First(p => p.BinCode == binCodeTo).WhsCode);
    }

    public async Task<string?> ItemCodeByGtinAsync(string gtin, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/BarCodes?$filter=Barcode eq '{gtin}'&$select=ItemNo");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await ItemCodeByGtinAsync(gtin, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"ItemCodeByGtin - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"ItemCodeByGtin status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var itemCode = JsonSerializer.Deserialize<ItemsCode>(json) ?? throw new ArgumentNullException("body service layer ItemCodeByGtin");

        if (!itemCode.Items.Any())
            return null;

        return itemCode.Items.First().ItemNo;
    }

    public async Task<string?> ItemCodeBySerieAsync(string serie, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SerialNumberDetails?$filter=SerialNumber eq '{serie}'&$select=ItemCode");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await ItemCodeBySerieAsync(serie, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"ItemCodeBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"ItemCodeBySerieAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var itemCode = JsonSerializer.Deserialize<ItemsCode>(json) ?? throw new ArgumentNullException("body service layer ItemCodeByGtin");

        if (!itemCode.Items.Any())
            return null;

        return itemCode.Items.First().ItemCode;
    }

    public async Task<string?> ItemCodeBySerieAsync(string serie, string bincode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('serieInBinCode')/List?serie='{serie}'&bincode='{bincode}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await ItemCodeBySerieAsync(serie, bincode, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"ItemCodeBySerieAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"ItemCodeBySerieAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var itemCode = JsonSerializer.Deserialize<ItemsCode>(json) ?? throw new ArgumentNullException("body service layer ItemCodeByGtin");

        if (!itemCode.Items.Any())
            return null;

        return itemCode.Items.First().ItemCode;
    }

    public async Task StockTransferAsync(Transfer transfer, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync($"/b1s/v1/StockTransfers", new StringContent(JsonSerializer.Serialize(transfer), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await StockTransferAsync(transfer, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"StockTransferAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"StockTransferAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<(int binAbsFrom, int binAbsTo, string whsCodeFrom, string whsCodeTo)> BinAbsByBinCode(string binCodeFrom, string binCodeTo, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('BinAbsFromBinCode')/List?binCodeFrom='{binCodeFrom}'&binCodeTo='{binCodeTo}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await BinAbsByBinCode(binCodeFrom, binCodeTo, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"BinAbsFromBinCode - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"BinAbsFromBinCode status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var bins = JsonSerializer.Deserialize<BinLocation>(json) ?? throw new ArgumentNullException("body service layer BinAbsFromBinCode");

        if (bins.Items.Count < 2)
            return default;

        return new((int)bins.Items.First(p => p.BinCode == binCodeFrom).AbsEntry,
                    (int)bins.Items.First(p => p.BinCode == binCodeTo).AbsEntry,
                    bins.Items.First(p => p.BinCode == binCodeFrom).WhsCode,
                    bins.Items.First(p => p.BinCode == binCodeTo).WhsCode);
    }

    public async Task<string?> ItemCodeByGtin(string gtin, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/Items?$filter=BarCode eq '{gtin}'&$select=ItemCode");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await ItemCodeByGtin(gtin, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"ItemCodeByGtin - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"ItemCodeByGtin status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var itemCode = JsonSerializer.Deserialize<ItemsCode>(json) ?? throw new ArgumentNullException("body service layer ItemCodeByGtin");

        if (!itemCode.Items.Any())
            return null;

        return itemCode.Items.First().ItemCode;
    }

    public async Task<Inventory?> InventoryBalanceSeriesByBinCodeAsync(string binCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer","odata.maxpagesize=500");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('SeriesByBinCode')/List?bincode='{binCode}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await InventoryBalanceSeriesByBinCodeAsync(binCode, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"InventoryBalanceSeriesByBinCodeAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<Inventory>(json);
    }

    public async Task<StockVirtual?> GetStockVirtualAsync(int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/StockVirtual");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetStockVirtualAsync(1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetStockVirtualAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<StockVirtual>(json);
    }

    public async Task<StockVirtual?> GetStockVirtualByDocEntryAsync(string docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/StockVirtual?$filter=Code eq '{docEntry}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetStockVirtualByDocEntryAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetStockVirtualByDocEntryAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<StockVirtual>(json);
    }

    public async Task<StockVirtual?> GetStockVirtualByItemCodeAsync(string docEntry, int tryLogin = 0)
    {

        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/StockVirtual?$filter=U_ItemCode eq '{docEntry}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetStockVirtualByItemCodeAsync(docEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetStockVirtualByItemCodeAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<StockVirtual>(json);
    }

    public async Task UpdateStockVirtualAsync(UpdateObj updateObj, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PatchAsync($"/b1s/v1/StockVirtual('{updateObj.Code}')", new StringContent(JsonSerializer.Serialize(updateObj), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateStockVirtualAsync(updateObj, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateStockVirtualAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"UpdateStockVirtualAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

    }

    public async Task DeleteStockVirtualAsync(string docEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.DeleteAsync($"/b1s/v1/StockVirtual('{docEntry}')");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await DeleteStockVirtualAsync(docEntry, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"DeleteStockVirtualAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"DeleteStockVirtualAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

    }

    public async Task PostStockVirtualAsync(UpdateObj updateObj, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.PostAsync($"/b1s/v1/StockVirtual", new StringContent(JsonSerializer.Serialize(updateObj), Encoding.UTF8, Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await PostStockVirtualAsync(updateObj, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"PostStockVirtualAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"PostStockVirtualAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
    }

    public async Task<StockByItem?> GetStockAllWarehouseAvaliable(string itemCode, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
        {
            return client.GetAsync($"/b1s/v1/SQLQueries('stock-by-item')/List?ItemCode='{itemCode}'");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetStockAllWarehouseAvaliable(itemCode, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetStockAllWarehouseAvaliable - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        return JsonSerializer.Deserialize<StockByItem>(json);
    }
}

