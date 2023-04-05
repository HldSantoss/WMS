using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities.Orders
{
    public class LateOrdersResult
    {
        [JsonPropertyName("value")]
        public List<LateOrders> LateOrder { get; set; }
    }

    public class LateOrders
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string CardName { get; set; }
        public string NumAtCard { get; set; }
        public string Comments { get; set; }
        public string UpdateDate { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public string U_WMS_Status { get; set; }
    }
}
