using Domain.Entities.ReceiptOfGoods;

namespace Domain.Services;

public interface IGoodsReceivingService
{
    Task<ReceivingWms> StartReceivingAsync(string keyAccess, string userName);
    Task ReceivingItemBySkuAsync(ReceivingItemWms item, long docEntry);
    Task ReceivingItemBySerieAsync(ReceivingItemBySerie item, long docEntry, int lineNum);
    Task UndoItemAsync(Item item, long docEntry);
    Task FinishReceivingAsync(string keyAccess, long docEntry, string userName);

    //Products Testing and Preparation
    Task<SeriesItem?> DanfeEquipamentAsync(string keyAccess);
    Task GetEquipamentTestAsync(string serialNumber, string userName);
    Task RecordSeriesItemReceivingAsync(SeriesItem seriesItem, long docEntry);
}