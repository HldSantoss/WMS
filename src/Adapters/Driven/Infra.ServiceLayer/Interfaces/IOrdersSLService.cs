using System;
using Domain.Entities;
using Domain.Entities.Orders;

namespace Infra.ServiceLayer.Interfaces
{
	public interface IOrdersSLService
	{
		Task<IEnumerable<SalesOrderSummary>> GetSummarySalesOrder(int brandId, int tryLogin = 0);
        Task<SalesOrderSummaryByStoreNew> GetSummarySalesOrderByStore(int brandId, int tryLogin = 0);
        Task<OrderSLSummary> GetDetailsSummarySalesOrderByStore(int brandId, string status, string store,  int tryLogin = 0);
        Task<ListCardCode> GetOrderClientByCPF(string cpf, string formattedCPF, int tryLogin = 0);
        Task<CardCodeByOrder> GetOrderClientByReference(string reference, int tryLogin = 0);
        Task<ListCardCode> GetOrderClientByDocNum(string docNum, int tryLogin = 0);
        Task<ListCardCode> GetOrderClientByOrderVTEX(string vtex, int tryLogin = 0);
        Task<ListCardCode> GetOrderClientByOrderML(string ml, int tryLogin = 0);
        Task<OrdersByCardCode> GetOrdersByCardCode(string cardCode, int tryLogin = 0);
        Task<OrderDocument> GetOrdersByDocEntry(string docEntry, int tryLogin = 0);
        Task<KeyAccess> GetKeyAccessByDocEntryOrder(long docEntry, int tryLogin = 0);
        Task<IEnumerable<SalesOrderByUfSummary?>> GetSummarySalesOrderByUf(int tryLogin = 0);
        Task<SummaryDetails> GetSummarySalesOrderDetails(string startAt, string finishAt, string status, int branchId, int tryLogin = 0);
        Task CancelOrderByDocEntry(long docEntry, int tryLogin = 0);
        Task UpdateOrder(long docEntry, string group, string status, string comments, int tryLogin = 0);
        Task<OpenOrdersMarketPlaceList> GetOpenMarketPlaceOrders(int tryLogin = 0);
        Task<OrderSAP> GetOrdersToDispachtInIntelipost(long packingListId, int tryLogin = 0);
        Task<OrderSAP> GetOrderToSendInIntelipost(long packingListId, int tryLogin = 0);
        Task<InfoDetails> GetInfoDetails(long docNum, int tryLogin = 0);
        Task<LateOrdersResult> GetLateOrders(int tryLogin = 0);
        Task<InvoiceSummary?> GetInvoiceSummaryAsync(string keyAccess, int tryLogin = 0);
    }
}