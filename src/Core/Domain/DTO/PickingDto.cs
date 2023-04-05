using System.Text.Json.Serialization;

namespace Domain.DTO
{
    public record PickingDto
    {
        public PickingDto(List<PickingValueDto> value)
        {
            Value = value;
        }

        [JsonPropertyName("value")]
        public List<PickingValueDto> Value { get; set; }
    }

    public record PickingValueDto
    {
        public PickingValueDto(string cardName, string carrier, int docEntry, string itemCode, int lineNum, string numAtCard, double quantity, string whsCode)
        {
            CardName = cardName;
            Carrier = carrier;
            DocEntry = docEntry;
            ItemCode = itemCode;
            LineNum = lineNum;
            NumAtCard = numAtCard;
            Quantity = quantity;
            WhsCode = whsCode;
        }

        public string CardName { get; set; }
        public string Carrier { get; set; }
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        public int LineNum { get; set; }
        public string NumAtCard { get; set; }
        public double Quantity { get; set; }
        public string WhsCode { get; set; }
    }
}