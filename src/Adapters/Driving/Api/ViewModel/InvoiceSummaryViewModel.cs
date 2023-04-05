using System.Text.Json.Serialization;

namespace Api.ViewModel
{
    public class InvoiceSummaryViewModel
    {
        public string nameClient { get; set; } 
        public string branch { get; set; }
        public string order { get; set; }
        public string danfe { get; set; }
        public List<DocumentLineSummaryLineViewModel> items { get; set; }
    }

    public class DocumentLineSummaryLineViewModel
    {
        public string sku { get; set; }
        public string description { get; set; }
        public double qty { get; set; }
    }

}
