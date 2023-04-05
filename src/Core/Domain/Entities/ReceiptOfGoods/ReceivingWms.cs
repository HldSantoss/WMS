namespace Domain.Entities.ReceiptOfGoods;

public class ReceivingWms
{
    public ReceivingWms(string nFeKey, string supplier, long docEntry, string? purchaseOrder, string status, DateTime? dt_schedule, string obs, string? agent, List<Item> items)
    {
        NFeKey = nFeKey;
        Supplier = supplier;
        DocEntry = docEntry;
        PurchaseOrder = purchaseOrder;
        Status = status;
        Dt_schedule = dt_schedule;
        Obs = obs;
        Agent = agent;
        Items = items;
    }

    public string NFeKey { get; set; }
    public string Supplier { get; set; }
    public long DocEntry { get; set; }
    public string? PurchaseOrder { get; set; }
    public string Status { get; set; }
    public DateTime? Dt_schedule { get; set; }
    public string Obs { get; set; }
    public string? Agent { get; set; }
    public List<Item> Items { get; set; }
}

public class ReceivingItemWms
{
    public string MaterialCode { get; set; }
    public int QuantityReceiving { get; set; }
}

public class Item
{
    public Item(int lineNum, double quantity, double quantityReceiving)
    {
        LineNum = lineNum;
        Quantity = quantity;
        QuantityReceiving = quantityReceiving;
    }

    public Item()
    {

    }

    public Item(string materialCode, string materialDescription, int lineNum, string ean, string? series, double quantity, double quantityReceiving, int weight, string serialControlled, string printerRange)
    {
        MaterialCode = materialCode;
        MaterialDescription = materialDescription;
        LineNum = lineNum;
        Ean = ean;
        Series = series;
        Quantity = quantity;
        QuantityReceiving = quantityReceiving;
        Weight = weight;
        SerialControlled = serialControlled;
        PrinterRange = printerRange;
    }

    public string? MaterialCode { get; set; }
    public string? MaterialDescription { get; set; }
    public int LineNum { get; set; }
    public string? Ean { get; set; }
    public string? Series { get; set; }
    public double Quantity { get; set; }
    public double QuantityReceiving { get; set; }
    public int Weight { get; set; }
    public string SerialControlled { get; set; }
    public string PrinterRange { get; set; }
}