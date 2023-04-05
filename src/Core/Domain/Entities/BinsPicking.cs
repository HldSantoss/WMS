using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public record BinsPicking
    {
        public BinsPicking(List<PickBins> itemsBins)
        {
            ItemsBins = itemsBins;
        }

        [JsonPropertyName("value")]
        public List<PickBins> ItemsBins { get; init; }
    }

    public record PickBins
    {
        public PickBins(int absEntry, string binCode, string itemCode, string itemName, double onHandQty, string whsCode, string manBtchNum, string manSerNum)
        {
            AbsEntry = absEntry;
            BinCode = binCode;
            ItemCode = itemCode;
            ItemName = itemName;
            OnHandQty = onHandQty;
            WhsCode = whsCode;
            ManBtchNum = manBtchNum;
            ManSerNum = manSerNum;
        }

        public int AbsEntry { get; init; }
        public string BinCode { get; init; }
        public string ItemCode { get; init; }
        public string ItemName { get; init; }
        public double OnHandQty { get; init; }
        public string WhsCode { get; init; }
        public string ManBtchNum { get; set; }
        public string ManSerNum { get; set; }
    }
}