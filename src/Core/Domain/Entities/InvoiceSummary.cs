using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DocumentLineSummary
    {
        [JsonPropertyName("ItemCode")]
        public string sku { get; set; }

        [JsonPropertyName("ItemDescription")]
        public string description { get; set; }

        [JsonPropertyName("Quantity")]
        public double qty { get; set; }
    }
    
    public class InvoiceSummary
    {
      public List<InvoiceSummaryLine> value { get; set; }
    }

    public class InvoiceSummaryLine
    {
        [JsonPropertyName("CardName")]
        public string nameClient { get; set; }

        [JsonPropertyName("BPLName")]
        public string branch { get; set; }

        [JsonPropertyName("NumAtCard")]
        public string order { get; set; }
   
        [JsonPropertyName("U_ChaveAcesso")]
        public string danfe { get; set; }

        [JsonPropertyName("DocumentLines")]
        public List<DocumentLineSummary> items { get; set; }
    }

}
