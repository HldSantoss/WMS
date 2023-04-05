using System;
using Domain.Entities;
using Domain.Entities.Inventories;

namespace Infra.ServiceLayer.Interfaces
{
	public interface IInventorySLService
	{
        Task<(int binAbsFrom, int binAbsTo, string whsCodeFrom, string whsCodeTo)> BinAbsByBinCodeAsync(string binCodeFrom, string binCodeTo, int tryLogin = 0);
        Task<string?> ItemCodeByGtinAsync(string gtin, int tryLogin = 0);
        Task<string?> ItemCodeBySerieAsync(string serie, int tryLogin = 0);
        Task<string?> ItemCodeBySerieAsync(string serie, string bincode, int tryLogin = 0);
        Task StockTransferAsync(Transfer transfer, int tryLogin = 0);
        Task<Inventory?> InventoryBalanceBinLocationsByItemCodeAsync(string sku, int tryLogin = 0);
        Task<Inventory?> InventoryBalanceBinLocationsBySerieAsync(string serie, int tryLogin = 0);
        Task<Inventory?> InventoryBalanceBinLocationsByBinCodeAsync(string binCode, int tryLogin = 0);
        Task<Inventory?> InventoryBalanceSeriesByBinCodeAsync(string binCode, int tryLogin = 0);
        Task<StockByItem?> GetStockAllWarehouseAvaliable(string itemCode, int tryLogin = 0);
        Task<StockVirtual?> GetStockVirtualAsync(int tryLogin = 0);
        Task<StockVirtual?> GetStockVirtualByDocEntryAsync(string docEntry, int tryLogin = 0);
        Task<StockVirtual?> GetStockVirtualByItemCodeAsync(string docEntry,int tryLogin = 0);
        Task PostStockVirtualAsync(UpdateObj updateObj, int tryLogin = 0);
        Task UpdateStockVirtualAsync(UpdateObj updateObj, int tryLogin = 0);
        Task DeleteStockVirtualAsync(string docEntry, int tryLogin = 0);
    }
}

