using System.Text.Json.Serialization;

namespace Domain.Entities.Inventories
{
    public record BinLocation
    {
        public BinLocation(List<BinLocationItems> items)
        {
            Items = items;
        }

        [JsonPropertyName("value")]
        public List<BinLocationItems> Items { get; init; }
    }

    public class BinLocationItems
    {
        public BinLocationItems(int absEntry, string binCode, string whsCode)
        {
            AbsEntry = absEntry;
            BinCode = binCode;
            WhsCode = whsCode;
        }

        public int AbsEntry { get; init; }
        public string BinCode { get; init; }
        public string WhsCode { get; init; }
    }
}