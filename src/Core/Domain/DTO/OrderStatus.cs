using System.Text.Json.Serialization;

namespace Domain.DTO
{
    public class OrderStatus
    {
        public OrderStatus(string status)
        {
            Status = status;
        }

        [JsonPropertyName("U_WMS_Status")]
        public string Status { get; set; }
    }
}