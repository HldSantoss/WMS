using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Branches
{
    public Branches(List<Branch> list)
    {
        List = list;
    }

    [JsonPropertyName("value")]
    public List<Branch> List { get; set; }
}

public class Branch
{
    public string? BPLName { get; set; }
    public int? BPLId { get; set; }
    public string? CNPJ { get; set; }
    public string? IE { get; set; }
    public string? AddrType { get; set; }
    public string? Street { get; set; }
    public string? StreetNo { get; set; }
    public string? ZipCode { get; set; }
    public string? Block { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
}