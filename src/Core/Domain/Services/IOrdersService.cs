using System;
using Domain.Entities;
using Domain.Entities.Orders;

namespace Domain.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<SalesOrdersTracking>> SalesOrdersTrackingAsync(int branchId);
        Task<IEnumerable<SalesOrderStore>> SalesOrdersTrackingByStoreAsync(int branchId);
        Task<IEnumerable<SalesOrdersTracking>> SalesOrdersByUfTrackingAsync();
        Task<IEnumerable<OrderDetails>> SalesOrdersDetailsAsync(string range, string status, int branchId);
        Task<OrdersByCardCode> SalesOrdersByClientAsync(string reference);
        Task CancelOrderByDocEntryAsync(long docEntry);
    }
}

