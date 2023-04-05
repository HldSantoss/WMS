using Domain.DTO;
using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces;

public interface IPickingSLService
{
    Task<long?> GetNextPickingAsync(string groupId, int tryLogin = 0);
    Task<Picking?> GetPickingAsync(long docEntry, int tryLogin = 0);
    Task<PickingUser?> GetPickingLoginAsync(long docEntry, int tryLogin = 0);
    Task<PickingGroup?> GetUsersPickingGroupAsync(int tryLogin = 0);
    Task UpdatePickingStatusAsync(string status, long docEntry, string user, string? sro = null, int tryLogin = 0);
    Task UpdatePickingStatusByCheckoutAsync(string status, long docEntry, int tryLogin = 0);
    Task UpdatePickingStatusReplenishAsync(string status, long docEntry, string itemCode, int tryLogin = 0);
    Task<BinsPicking?> GetBinsPickingAsync(long docEntry, int tryLogin = 0);
    Task<long> GetDocEntryPickSavedAsync(long docEntry, int tryLogin = 0);
    Task<SavedPickingDto?> GetSavedPickingAsync(long docEntry, int tryLogin = 0);
    Task RemovePickSavedAsync(long pickEntry, int tryLogin = 0);
    Task CreatePickSavedAsync(Picking picking, int tryLogin = 0);
    Task<BinsPicking?> SuggestNextBinPickingAsync(string itemCode, string binCode, int tryLogin = 0);
    Task<bool> KeepGoingAsync(long docEntry, int tryLogin = 0);
    Task<string> GetCarrierName(string carrier);
    Task<ErrorIntegration?> GetErrorDetails(int tryLogin = 0);

}