using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods;

public class ScheduleReceiving
{
    [JsonPropertyName("odata.metadata")]
    public string odatametadata { get; set; }
    public List<SchedulingReceiving> value { get; set; }
}

public class SchedulingReceiving
{
    public int DocEntry { get; set; }
    public int DocNum { get; set; }
    public string DocDueDate { get; set; }
    public string CardName { get; set; }
    public string NumAtCard { get; set; }
    public string Comments { get; set; }
    public string U_WMS_Status { get; set; }
}