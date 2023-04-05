using Domain.Entities;

namespace Domain.Services
{
    public interface IPickingService
    {
        Task<Picking> GetPicking(string userId);
        Task<Picking?> ContinuePicking(long docEntry);
        Task FinishPickingAsync(Picking picking);
        Task SavePickingAsync(Picking picking);
        Task<IEnumerable<BinLocations>?> SuggestNextBinPickingAsync(string itemCode, string binCode, int lineNum, double remainingQuantity);
    }
}