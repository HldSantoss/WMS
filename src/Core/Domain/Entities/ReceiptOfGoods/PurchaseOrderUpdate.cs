using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods
{
    public record PurchaseOrderUpdate
    {
        public PurchaseOrderUpdate(List<PurchaseOrderUpdateDocumentLine> documentLines)
        {
            DocumentLines = documentLines;
        }

        public PurchaseOrderUpdate(string login, string status)
        {
            Login = login;
            Status = status;
        }

        public PurchaseOrderUpdate(string login, string status, List<PurchaseOrderUpdateDocumentLine> documentLines)
        {
            Login = login;
            Status = status;
            DocumentLines = documentLines;
        }

        [JsonPropertyName("U_CT_LoginWms")]
        public string? Login { get; set; }

        [JsonPropertyName("U_WMS_Status")]
        public string? Status { get; set; }

        [JsonPropertyName("DocumentLines")]
        public List<PurchaseOrderUpdateDocumentLine>? DocumentLines { get; set; }
    }

    public class PurchaseOrderUpdateDocumentLine
    {
        public PurchaseOrderUpdateDocumentLine(int lineNum, string quantity)
        {
            LineNum = lineNum;
            Quantity = quantity;
        }

        [JsonPropertyName("LineNum")]
        public int LineNum { get; set; }

        [JsonPropertyName("U_WMS_Qty_Receiving")]
        public string Quantity { get; set; }
    }

    public record PurchaseOrderUpdateHeader
    {
        public PurchaseOrderUpdateHeader(string login, string status)
        {
            Login = login;
            Status = status;
        }

        [JsonPropertyName("U_CT_LoginWms")]
        public string Login { get; set; }

        [JsonPropertyName("U_WMS_Status")]
        public string Status { get; set; }
    }
}