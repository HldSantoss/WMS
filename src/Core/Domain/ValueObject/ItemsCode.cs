using System.Text.Json.Serialization;

namespace Domain.ValueObject
{
    public record ItemsCode
    {
        public ItemsCode(List<ItemsCodeValue> items)
        {
            Items = items;
        }

        [JsonPropertyName("value")]
        public List<ItemsCodeValue> Items { get; init; }
    }

    public record ItemsCodeValue
    {
        public ItemsCodeValue(string itemCode, string itemNo)
        {
            ItemCode = itemCode;
            ItemNo = itemNo;
        }

        public string ItemNo { get; init; }
        public string ItemCode { get; init; }
    }
}