using Domain.Adapters;
using Domain.Entities.Inventories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;

namespace Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IInventorySLService _inventorySLService;

        public TransferService(IInventorySLService inventorySLService)
        {
            _inventorySLService = inventorySLService;
        }

        public async Task<bool> TranferItemByGtinAsync(string gtin, double quantity, string binCodeFrom, string binCodeTo)
        {
            var dataBins = await _inventorySLService.BinAbsByBinCodeAsync(binCodeFrom, binCodeTo);
            var itemCode = await _inventorySLService.ItemCodeByGtinAsync(gtin);

            if (itemCode == null || dataBins == default)
                return false;
            
            var transfer = new Transfer(dataBins.whsCodeFrom, dataBins.whsCodeTo, new List<StockTransferLine>
            {
                new StockTransferLine(itemCode, quantity.ToString(),  dataBins.whsCodeTo, dataBins.whsCodeFrom, new List<StockTransferLinesBinAllocation>
                {
                    new StockTransferLinesBinAllocation(dataBins.binAbsFrom, quantity, "tNO", -1, "batFromWarehouse", 0),
                    new StockTransferLinesBinAllocation(dataBins.binAbsTo, quantity, "tNO", -1, "batToWarehouse", 0)
                }, new List<SerialNumbers>())
            });

            await _inventorySLService.StockTransferAsync(transfer);
            return true;
        }

        public async Task<bool> TranferItemBySerieAsync(string serie, double quantity, string binCodeFrom, string binCodeTo)
        {
            var dataBins = await _inventorySLService.BinAbsByBinCodeAsync(binCodeFrom, binCodeTo);
            var itemCode = await _inventorySLService.ItemCodeBySerieAsync(serie);

            if (itemCode == null || dataBins == default)
                return false;
            
            var transfer = new Transfer(dataBins.whsCodeFrom, dataBins.whsCodeTo, new List<StockTransferLine>
            {
                new StockTransferLine(itemCode, quantity.ToString(), dataBins.whsCodeTo, dataBins.whsCodeFrom, new List<StockTransferLinesBinAllocation>
                {
                    new StockTransferLinesBinAllocation(dataBins.binAbsFrom, quantity, "tNO", 0, "batFromWarehouse", 0),
                    new StockTransferLinesBinAllocation(dataBins.binAbsTo, quantity, "tNO", 0, "batToWarehouse", 0)
                }, new List<SerialNumbers>
                {
                    new SerialNumbers(serie, 0, quantity)
                })
            });

            await _inventorySLService.StockTransferAsync(transfer);

            return true;
        }

        public async Task<bool> TranferItemByItemCodeAsync(string itemCode, double quantity, string binCodeFrom, string binCodeTo)
        {
            var dataBins = await _inventorySLService.BinAbsByBinCodeAsync(binCodeFrom, binCodeTo);

            if (itemCode == null || dataBins == default)
                return false;
            
            var transfer = new Transfer(dataBins.whsCodeFrom, dataBins.whsCodeTo, new List<StockTransferLine>
            {
                new StockTransferLine(itemCode, quantity.ToString(), dataBins.whsCodeTo, dataBins.whsCodeFrom, new List<StockTransferLinesBinAllocation>
                {
                    new StockTransferLinesBinAllocation(dataBins.binAbsFrom, quantity, "tNO", -1, "batFromWarehouse", 0),
                    new StockTransferLinesBinAllocation(dataBins.binAbsTo, quantity, "tNO", -1, "batToWarehouse", 0)
                }, new List<SerialNumbers>())
            });

            await _inventorySLService.StockTransferAsync(transfer);

            return true;
        }

        public async Task<Inventory?> InventoryBalanceBinLocationsByGtinServiceAsync(string gtin, int tryLogin = 0)
        {
            var itemCode = await _inventorySLService.ItemCodeByGtinAsync(gtin);

            if (itemCode == null)
                return null;

            return await _inventorySLService.InventoryBalanceBinLocationsByItemCodeAsync(itemCode);
        }
    }
}