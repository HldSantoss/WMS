using System.Text.Json.Serialization;

namespace Domain.Entities;

public class SroData
{
    public string Code { get; set; }
    
    [JsonPropertyName("U_End")]
    public int End { get; set; }

    [JsonPropertyName("U_Current")]
    public int Current { get; set; }

    [JsonPropertyName("U_Prefix")]
    public string Prefix { get; set; }

    [JsonPropertyName("U_Suffix")]
    public string Suffix { get; set; }

    [JsonPropertyName("U_Contract")]
    public string Contract { get; set; }
}