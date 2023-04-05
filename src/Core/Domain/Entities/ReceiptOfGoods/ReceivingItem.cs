using System;
using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods
{
    public class ReceivingItem
    {
        public ReceivingItem()
        {
        }

        public ReceivingItem(List<ReceivingItemLine> value)
        {
            Value = value;
        }

        [JsonPropertyName("value")]
        public List<ReceivingItemLine> Value { get; set; }
    }


    public class ReceivingItemLine
    {
        public ReceivingItemLine()
        {

        }

        public ReceivingItemLine(int lineNum, string itemCode, double quantity, string quantityReceiving, string manSerNum)
        {
            LineNum = lineNum;
            ItemCode = itemCode;
            Quantity = quantity;
            QuantityReceiving = quantityReceiving;
            ManSerNum = manSerNum;
        }

        public int LineNum { get; set; }

        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        [JsonPropertyName("U_WMS_Qty_Receiving")]
        public string QuantityReceiving { get; set; }

        public string ManSerNum { get; set; }
    }
}
