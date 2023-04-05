using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Domain.Entities;
using Domain.Entities.Orders;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.ServiceLayer.Operations
{
	public class OrdersSLService : IOrdersSLService
	{
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private readonly ILoginSLService _loginService;
        private readonly ILogger<GoodsReceivingSLService> _logger;

        public OrdersSLService(IConfiguration configuration,
                               IHttpClientFactory httpClientFactory,
                               AsyncCircuitBreakerPolicy circuitBreaker,
                               ILoginSLService loginService,
                               ILogger<GoodsReceivingSLService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _circuitBreaker = circuitBreaker;
            _loginService = loginService;
            _logger = logger;
        }

        public async Task<ListCardCode?> GetOrderClientByDocNum(string docNum, int tryLogin = 0)
        {
            var limitDate = DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd");
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('search-client-by-cpf')/List?cpf='{limitDate}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByDocNum(docNum, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrderClientBy - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<ListCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<ListCardCode> GetOrderClientByCNPJ(string cnpj, int tryLogin = 0)
        {
            var limitDate = DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd");
            var client = _httpClientFactory.CreateClient("ServiceLayer");
         
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('search-client-by-cnpj')/List?cnpj='{cnpj}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByCNPJ(cnpj, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrderClientByCNPJ - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<ListCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<ListCardCode> GetOrderClientByCPF(string cpf, string formattedCPF, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
       
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('search-client-by-cpf')/List?formattedCPF='{formattedCPF}'&unformattedCPF='{cpf}'&formattedCPF='{formattedCPF}'&unformattedCPF='{cpf}'&unformattedCPF='{cpf}'&formattedCPF='{formattedCPF}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByCPF(cpf, formattedCPF, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrderClientByCPF - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<ListCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<ListCardCode> GetOrderClientByOrderML(string ml, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
           
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('search-client-by-ml')/List?ml='{ml}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByOrderML(ml, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrderClientByOrderML - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<ListCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<ListCardCode> GetOrderClientByOrderVTEX(string vtex, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
          
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('search-client-by-vtex')/List?vtex='{vtex}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByOrderVTEX(vtex, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrderClientByOrderVTEX - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<ListCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<IEnumerable<SalesOrderSummary>> GetSummarySalesOrder(int brandId, int tryLogin = 0)
        {
            var limitDate = DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd");
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('summary-order')/List?dateReference='{limitDate}'&branchId={brandId}");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetSummarySalesOrder(1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetSummarySalesOrder - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<SalesOrderSummaryList>(json) ?? throw new ArgumentNullException("body service layer");

            return lists.SalesOrders;

        }

        public async Task<IEnumerable<SalesOrderByUfSummary?>> GetSummarySalesOrderByUf(int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('summary-order-uf')/List");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetSummarySalesOrderByUf(1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetSummarySalesOrderByUf - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<SalesOrderByUfSummaryList>(json) ?? throw new ArgumentNullException("body service layer");

            return lists.SalesOrders;

        }

        public async Task<SummaryDetails> GetSummarySalesOrderDetails(string startAt, string finishAt, string status, int branchId, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('DetailSummary')/List?incial='{startAt}'&final='{finishAt}'&status='{status}'&bplId={branchId}");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetSummarySalesOrderDetails(startAt, finishAt, status, branchId, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetSummarySalesOrderDetails - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<SummaryDetails>(json) ?? throw new ArgumentNullException("body service layer");

        }

        public async Task<OrdersByCardCode> GetOrdersByCardCode(string cardCode, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=30");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=(CardCode eq '{cardCode}')&$select=DocEntry,NumAtCard,CardName,U_WMS_Status");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrdersByCardCode(cardCode, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrdersByCardCode - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<OrdersByCardCode>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<OrderDocument> GetOrdersByDocEntry(string docEntry, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=30");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders({docEntry})");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrdersByDocEntry(docEntry, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrdersByDocEntry - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<OrderDocument>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task CancelOrderByDocEntry(long docEntry, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
               return client.PostAsync($"/b1s/v1/Orders({docEntry})/Cancel", null);
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                await CancelOrderByDocEntry(docEntry, 1);
                return;
            }

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception($"CancelOrderByDocEntry - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
        }

        public async Task<KeyAccess> GetKeyAccessByDocEntryOrder(long docEntry, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('keyAccessByDocEntry')/List?docentry={docEntry}");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetKeyAccessByDocEntryOrder(docEntry, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"docEntry - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<KeyAccess>(json) ?? throw new ArgumentNullException("body service layer");
        }

        public async Task<SalesOrderSummaryByStoreNew> GetSummarySalesOrderByStore(int brandId, int tryLogin = 0)
        {
            var limitDate = DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd");
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/SQLQueries('summary-order-store')/List?bplId={brandId}");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetSummarySalesOrderByStore(brandId, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetSummarySalesOrderByStore - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<SalesOrderSummaryByStoreNew>(json) ?? throw new ArgumentNullException("body service layer");

        }

        public async Task<OpenOrdersMarketPlaceList> GetOpenMarketPlaceOrders(int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=(U_CT_Store eq '7' and U_WMS_Status ne 'Shipped')&$select=DocEntry,DocNum,DocumentStatus,NumAtCard,CardName, Comments,CreationDate,U_AS_NUMPED,U_WMS_Status,U_PickingGroup,U_CT_Label,Cancelled,U_CT_PackingListId,UpdateDate,UpdateTime");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOpenMarketPlaceOrders(1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOpenMarketPlaceOrders - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<OpenOrdersMarketPlaceList>(json) ?? throw new ArgumentNullException("body service layer");         
        }

        public async Task UpdateOrder(long docEntry, string group, string status, string comments, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "return-no-content");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PatchAsync($"/b1s/v1/Orders({docEntry})",
                                     new StringContent(JsonSerializer.Serialize(new { U_WMS_Status = status, U_PickingGroup = group, Comments = comments }),
                                                       Encoding.UTF8,
                                                       Application.Json));
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                await UpdateOrder(docEntry, group, status, comments, 1);
                return;
            }

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception($"UpdateOrder - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        }

        public async Task<OrderSLSummary> GetDetailsSummarySalesOrderByStore(int brandId, string status, string store, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=(Cancelled eq 'tNO' and  BPL_IDAssignedToInvoice eq {brandId} and U_WMS_Status eq '{status}' and U_CT_Store eq '{store}')&$select=DocEntry,DocNum,NumAtCard,U_AS_NUMPED,CreationDate,DocDate,CardCode,CardName,DocTotal,U_WMS_Status,U_CT_Store, U_PickingGroup, Comments,U_CT_LoginWms");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetDetailsSummarySalesOrderByStore(brandId, status, store, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetDetailsSummarySalesOrderByStore - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<OrderSLSummary>(json) ?? throw new ArgumentNullException("body service layer");

        }

        public async Task<CardCodeByOrder> GetOrderClientByReference(string reference, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=500");
            string r;
            if(reference.Substring(0,2) == "OP" || reference.Contains("-"))
            {
                r = $"NumAtCard eq '{reference}'";
            }
            else
            {
                r = $"NumAtCard eq '{reference}' or DocEntry eq {reference}";
            }

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=({r} or U_AS_NUMPED eq '{reference}')&$top=1&$select=CardCode");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrderClientByReference(reference, 1);
            }   

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetDetailsSummarySalesOrderByStore - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<CardCodeByOrder>(json) ?? throw new ArgumentNullException("body service layer");
        }

        public async Task<OrderSAP> GetOrdersToDispachtInIntelipost(long packingListId, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=(U_CT_PackingListId eq '{packingListId}')&$top=200");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrdersToDispachtInIntelipost(packingListId, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrdersToDispachtInIntelipost - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<OrderSAP>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<InfoDetails> GetInfoDetails(long docNum, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.GetAsync($"/b1s/v1/SQLQueries('get-info-invoices-by-docentry')/List?docNum={docNum}");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetInfoDetails(docNum, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
            return JsonSerializer.Deserialize<InfoDetails>(json) ?? throw new ArgumentNullException("body service layer");
        }

        public async Task<OrderSAP> GetOrderToSendInIntelipost(long packingListId, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=200");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Orders?$filter=(DocEntry eq {packingListId})&$top=1");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetOrdersToDispachtInIntelipost(packingListId, 1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetOrdersToDispachtInIntelipost - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            var lists = JsonSerializer.Deserialize<OrderSAP>(json) ?? throw new ArgumentNullException("body service layer");

            return lists;
        }

        public async Task<LateOrdersResult> GetLateOrders(int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=300");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.GetAsync($"/b1s/v1/Orders?$select=BPL_IDAssignedToInvoice,UpdateDate,DocNum,DocEntry,NumAtCard,CardName,U_WMS_Status,Comments,DocDate&$filter=DocumentStatus ne 'bost_Close'  and U_WMS_Status eq 'CanPick' or U_WMS_Status eq 'Created' or U_WMS_Status eq 'Replenish' or U_WMS_Status eq 'CanTax' or U_WMS_Status eq 'CanTaxValidation' and Cancelled ne 'tNO'&$orderby=DocDate");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetLateOrders(1);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
            return JsonSerializer.Deserialize<LateOrdersResult>(json) ?? throw new ArgumentNullException("body service layer");
        }

        public async Task<InvoiceSummary?> GetInvoiceSummaryAsync(string keyAccess, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");

            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() => {
                return client.GetAsync($"/b1s/v1/Invoices/?$top=1&$filter=U_ChaveAcesso eq '{keyAccess}'");
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                return await GetInvoiceSummaryAsync(keyAccess, tryLogin);
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"GetInvoiceSummaryAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"IServiceLayerAdapter status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");

            return JsonSerializer.Deserialize<InvoiceSummary>(json) ?? throw new ArgumentNullException("body service layer");
        }
    }
}

