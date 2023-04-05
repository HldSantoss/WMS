using Domain.Entities;

namespace Domain.Services
{
    public interface ICheckoutService
    {
        Task<Picking?> GetOrderAsync(long orderEntry, string userId = null);
        Task FinishAsync(long orderEntry, string userId = null);
    }
}