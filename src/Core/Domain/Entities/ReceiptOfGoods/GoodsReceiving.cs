using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods
{
    public class GoodsReceiving
    {
        public GoodsReceiving(IEnumerable<GoodsReceivingItem> item)
        {
            Item = item;
        }

        [JsonPropertyName("value")]
        public IEnumerable<GoodsReceivingItem> Item { get; set; }
    }

    public class GoodsReceivingItem
    {
        public GoodsReceivingItem(string canceled, 
            int bpl,
            string cardCode,
            string codeBars,
            string docDate, 
            string docDueDate, 
            long docEntry, 
            int docNum,
            int lineNum, 
            string docStatus, 
            string dscription, 
            string itemCode, 
            string keyaccess, 
            double quantity, 
            string quantityReceiving, 
            string status, 
            string comments,
            string manSerNum)
        {
            Canceled = canceled;
            Bpl = bpl;
            CardCode = cardCode;
            CodeBars = codeBars;
            DocDate = docDate;
            DocDueDate = docDueDate;
            DocEntry = docEntry;
            DocNum = docNum;
            LineNum = lineNum;
            DocStatus = docStatus;
            Dscription = dscription;
            ItemCode = itemCode;
            Keyaccess = keyaccess;
            Quantity = quantity;
            QuantityReceiving = quantityReceiving;
            Status = status;
            Comments = comments;
            ManSerNum = manSerNum;
        }

        [JsonPropertyName("CANCELED")]
        public string Canceled { get; set; }

        [JsonPropertyName("bpl")]
        public int Bpl { get; set; }
        public string CardCode { get; set; }
        public string CodeBars { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public long DocEntry { get; set; }
        public int DocNum { get; set; }
        public int LineNum { get; set; }
        public string DocStatus { get; set; }
        public string Dscription { get; set; }
        public string ItemCode { get; set; }
        public string Keyaccess { get; set; }
        public double Quantity { get; set; }
        public string QuantityReceiving { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public string ManSerNum { get; set; }
    }
}