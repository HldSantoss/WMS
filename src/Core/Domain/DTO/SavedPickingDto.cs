using System.Text.Json.Serialization;

namespace Domain.DTO
{
    public record SavedPickingDto
    {
        public SavedPickingDto(List<PickingSaved> pickings)
        {
            Pickings = pickings;
        }

        [JsonPropertyName("value")]
        public List<PickingSaved> Pickings { get; init; }
    }

    public record PickingSaved
    {
        public PickingSaved(string u_Payload, long docEntry)
        {
            U_Payload = u_Payload;
            DocEntry = docEntry;
        }

        public string U_Payload { get; init; }
        public long DocEntry { get; init; }
    }
}