using Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{   
    public class ErrorDetails
    {
        public string Creator { get; set; }
        public string Remark { get; set; }
        public string CreateDate { get; set; }
        public string U_Details { get; set; }
        public string U_OrderId { get; set; }
    }

    public class ErrorIntegration
    {
        [JsonPropertyName("value")]
        public List<ErrorDetails> Errors { get; set; }
    }
}
