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

public class CheckoutSLService : ICheckoutSLService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ILoginSLService _loginService;
    private readonly ILogger<CheckoutSLService> _logger;

    public CheckoutSLService(IConfiguration configuration,
                             IHttpClientFactory httpClientFactory,
                             AsyncCircuitBreakerPolicy circuitBreaker,
                             ILoginSLService loginService,
                             ILogger<CheckoutSLService> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _circuitBreaker = circuitBreaker;
        _loginService = loginService;
        _logger = logger;
    }

    public async Task<long?> GetInvoiceEntry(long orderEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/SQLQueries('invoiceEntryByDocEntry')/List?docEntry={orderEntry}");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetInvoiceEntry(orderEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetInvoiceEntry - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        var docEntry = result?["value"]?[0]?["DocEntry"]?.ToString();

        if (string.IsNullOrWhiteSpace(docEntry))
            return null;
        
        return Convert.ToInt64(docEntry);
    }

    public async Task<Picking?> GetInvoiceAsync(long invoiceEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Invoices({invoiceEntry})");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetInvoiceAsync(invoiceEntry, 1);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"GetInvoiceAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("Pedido inexistente");
        return JsonSerializer.Deserialize<Picking>(json) ?? throw new ArgumentNullException("Pedido inexistente");
    }

    public async Task<(string?, string?)> GetLabel(long orderEntry, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.GetAsync($"/b1s/v1/Orders({orderEntry})?$select=U_CT_Label,U_WM_TagDanfe");
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            return await GetLabel(orderEntry, 1);
        }

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("response login service layer");
        var result = JsonNode.Parse(json) ?? throw new ArgumentNullException("response login service layer");
        var labelML = result["U_CT_Label"];
        var labelDanfe = result["U_WM_TagDanfe"] ;

        return (labelML?.ToString() , labelDanfe?.ToString());
    }

    public async Task UpdateCheckoutStatusAsync(string status, long docEntry, string? sro = null, int tryLogin = 0)
    {
        var client = _httpClientFactory.CreateClient("ServiceLayer");
        var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
            return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status, U_CT_TrackingCode = sro }),
                                                       Encoding.UTF8,
                                                       Application.Json));
        });

        if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
        {
            await _loginService.LoginAsync();
            await UpdateCheckoutStatusAsync(status, docEntry, sro, 1);
            return;
        }

        if (response.StatusCode != HttpStatusCode.NoContent)
            throw new Exception($"UpdateCheckoutStatusAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

    }
}
