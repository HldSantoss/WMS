using Domain.Entities.ReceiptOfGoods;
using Domain.Entities.Inventories;
using Domain.Entities;
using Domain.DTO;
namespace Infra.ServiceLayer.Interfaces;

public interface IGoodsReceivingSLService
{
    // Order
    Task<SerialNumbersObj?> GetSerialNumbersObjBySerieAsync(string serial, int tryLogin = 0);
    Task<GoodsReceiving?> GetPurchaseOrderByKeyAcessAsync(string keyaccess, int tryLogin = 0);
    Task UpdateSerialPatrimonyBySerial(UpdateSerialNumber updateSerialNumber, string docEntry, int tryLogin = 0);
    Task<ReceivingItem?> GetQuantityReceivingPurchaseOrderByDocEntryAsync(long docEntry, int tryLogin = 0);
    Task UpdateItemByPurchaseOrder(PurchaseOrderUpdate purchaseOrder, long docEntry, int tryLogin = 0);
    Task UpdateHeaderPurchaseOrder(PurchaseOrderUpdateHeader purchaseOrder, long docEntry, int tryLogin = 0);
    Task CreateDeliveryNote(DeliveryNotesCreated deliveryNotesCreated, int tryLogin = 0);
    Task UpdateSerialNumbersObjBySerieAsync(Preparation preparation, long docEntry, int tryLogin = 0);
    //Scheduling
    Task<ScheduleReceiving?> GetSchedulingPurchaseOrderByDateAsync(DateTime startAt, DateTime finishAt, int tryLogin = 0);
    // Series
    Task PostSeriesItemAsync(SeriesItem seriesItem, int tryLogin = 0);
    Task UpdateSeriesItemHeaderAsync(SeriesItemHeader seriesItemHeader, long docEntry, int tryLogin = 0);
    Task UpdateSeriesItemLineAsync(SeriesItem seriesItem, long docEntry, int tryLogin = 0);
    Task<SeriesItem?> GetSeriesItemAsync(long docEntry, int tryLogin = 0);
    Task<bool> KeepGoingAsync(long docEntry, int tryLogin = 0);
}