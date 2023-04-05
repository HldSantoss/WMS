using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities.Inventories
{
    public class Inventory
    {
        public Inventory(IEnumerable<InventoryValue> value, string odataNextLink)
        {
            Value = value;
            OdataNextLink = odataNextLink;
        }

        [JsonPropertyName("value")]
        public IEnumerable<InventoryValue> Value { get; set; }
        public string OdataNextLink { get; set; }
    }

    public class InventoryValue
    {
        public InventoryValue(string distNumber, string binCode, string itemCode, string itemName, double onHandQty, RtrictType rtrictType)
        {
            DistNumber = distNumber;
            BinCode = binCode;
            ItemCode = itemCode;
            ItemName = itemName;
            OnHandQty = onHandQty;
            RtrictType = rtrictType;
        }

        public string? DistNumber { get; set; }
        public string BinCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double OnHandQty { get; set; }
        public RtrictType RtrictType { get; set; }
    }
}