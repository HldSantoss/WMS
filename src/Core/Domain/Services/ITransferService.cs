using Domain.Entities.Inventories;

namespace Domain.Services
{
    public interface ITransferService
    {
        Task<bool> TranferItemByGtinAsync(string gtin, double quantity, string binCodeFrom, string binCodeTo);
        Task<bool> TranferItemBySerieAsync(string serie, double quantity, string binCodeFrom, string binCodeTo);
        Task<bool> TranferItemByItemCodeAsync(string itemCode, double quantity, string binCodeFrom, string binCodeTo);
        Task<Inventory?> InventoryBalanceBinLocationsByGtinServiceAsync(string gtin, int tryLogin = 0);
    }
}