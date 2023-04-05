using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public record DocEntryByQuery
    {
        public DocEntryByQuery(List<DocEntryValue> docEntries)
        {
            DocEntries = docEntries;
        }

        [JsonPropertyName("value")]
        public List<DocEntryValue> DocEntries { get; set; }
    }

    public record DocEntryValue
    {
        public DocEntryValue(long docEntry)
        {
            DocEntry = docEntry;
        }

        public long DocEntry { get; set; }
    }
}