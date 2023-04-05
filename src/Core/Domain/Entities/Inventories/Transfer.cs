namespace Domain.Entities.Inventories
{
    public record Transfer
    {
        public Transfer(string fromWarehouse, string toWarehouse, List<StockTransferLine> stockTransferLines)
        {
            FromWarehouse = fromWarehouse;
            ToWarehouse = toWarehouse;
            StockTransferLines = stockTransferLines;
        }

        public string FromWarehouse { get; init; }
        public string ToWarehouse { get; init; }
        public List<StockTransferLine> StockTransferLines { get; init; }
    }

    public record StockTransferLine
    {
        public StockTransferLine(string itemCode, string quantity, string warehouseCode, string fromWarehouseCode,  List<StockTransferLinesBinAllocation> stockTransferLinesBinAllocations, List<SerialNumbers> serialNumbers)
        {
            ItemCode = itemCode;
            Quantity = quantity;
            WarehouseCode = warehouseCode;
            FromWarehouseCode = fromWarehouseCode;
            StockTransferLinesBinAllocations = stockTransferLinesBinAllocations;
            SerialNumbers = serialNumbers;
        }

        public string ItemCode { get; init; }
        public string Quantity { get; init; }
        public string WarehouseCode { get; init; }
        public string FromWarehouseCode { get; init; }
        public List<StockTransferLinesBinAllocation> StockTransferLinesBinAllocations { get; init; }
        public List<SerialNumbers> SerialNumbers { get; set; }
    }

    public record StockTransferLinesBinAllocation
    {
        public StockTransferLinesBinAllocation(int binAbsEntry, double quantity, string allowNegativeQuantity, int serialAndBatchNumbersBaseLine, string binActionType, int baseLineNumber)
        {
            BinAbsEntry = binAbsEntry;
            Quantity = quantity;
            AllowNegativeQuantity = allowNegativeQuantity;
            SerialAndBatchNumbersBaseLine = serialAndBatchNumbersBaseLine;
            BinActionType = binActionType;
            BaseLineNumber = baseLineNumber;
        }

        public int BinAbsEntry { get; init; }
        public double Quantity { get; init; }
        public string AllowNegativeQuantity { get; init; }
        public int SerialAndBatchNumbersBaseLine { get; init; }
        public string BinActionType { get; init; }
        public int BaseLineNumber { get; init; }
    }

    public record SerialNumbers
    {
        public SerialNumbers(string internalSerialNumber, int baseLineNumber, double quantity)
        {
            InternalSerialNumber = internalSerialNumber;
            BaseLineNumber = baseLineNumber;
            Quantity = quantity;
        }

        public string InternalSerialNumber { get; init; }
        public int BaseLineNumber { get; init; }
        public double Quantity { get; init; }
    }
}