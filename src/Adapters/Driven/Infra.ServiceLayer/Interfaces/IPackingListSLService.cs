using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces;

public interface IPackingListSLService
{
    Task<PackingList> CreatePackingListAsync(string carrierId, string method, int branch, int tryLogin = 0);
    Task<PackingList?> GetPackingListAsync(long docEntry, int tryLogin = 0);
    Task<PackingListRoot?> GetAllPackingListAsync(DateTime startAt, DateTime finishAt, string status, string bplId, int tryLogin = 0);
    Task<InfoNFPeerValidation?> GetInfoNFEAsync(string keyNfe, int tryLogin = 0);
    Task UpdateOrderDispached(long docEntry, int tryLogin = 0);
    Task UpdateOrderByPackingList(long docEntry, string packingListId, int tryLogin = 0);
    Task RemovePackingListAsync(long docEntry, int tryLogin = 0);
    Task<string?> ClosePackingListAsync(long docEntry, int tryLogin = 0);
    Task<string?> UpdateDateClosePackingListAsync(long docEntry, int tryLogin = 0);
    Task UpdateDateDispatchPackingListAsync(long docEntry, int tryLogin = 0);
    Task<(string NumAtCard, long DocEntry, long DocNum, string CardName, long SequenceSerial, string SeriesStr)> GetInvoiceEntryAsync(string keyNfe, int tryLogin = 0);
    Task AddItemPackingList(long packingListEntry, PackingListUpsert itemPackList, int tryLogin = 0);
    Task RemoveItemPackingListAsync(long packingListEntry, PackingList packingList, int tryLogin = 0);

    Task<(string AccessKey, string NumAtCard, long DocEntry, long DocNum, string CardName, long SequenceSerial, string SeriesStr)> GetInvoiceEntryBySerialAsync(string keyNfe, int tryLogin = 0);
}