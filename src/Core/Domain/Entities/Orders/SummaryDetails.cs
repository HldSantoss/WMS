using System.Text.Json.Serialization;

namespace Domain.Entities.Orders
{
    public class SummaryDetails
    {
        public SummaryDetails(List<OrderDetails> ordersDetails)
        {
            OrdersDetails = ordersDetails;
        }

        public SummaryDetails()
        {
        }

        [JsonPropertyName("value")]
        public List<OrderDetails> OrdersDetails { get; set; }
    }

    public class OrderDetails
    {
        public OrderDetails(string cardName, string carrier, int docEntry, int docNum, string numAtCard, string taxDate, string u_PickingGroup, string u_WMS_Status)
        {
            CardName = cardName;
            Carrier = carrier;
            DocEntry = docEntry;
            DocNum = docNum;
            NumAtCard = numAtCard;
            TaxDate = taxDate;
            U_PickingGroup = u_PickingGroup;
            U_WMS_Status = u_WMS_Status;
        }

        public OrderDetails()
        {
        }

        public string CardName { get; set; }
        public string Carrier { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string NumAtCard { get; set; }
        public string TaxDate { get; set; }
        public string U_PickingGroup { get; set; }
        public string U_WMS_Status { get; set; }
    }

    public class SalesOrderByParameter
    {
        public SalesOrderByParameter(string numberOrder, string nameClient, string status, DateTime taxDate)
        {
            NumberOrder = numberOrder;
            NameClient = nameClient;
            Status = status;
            TaxDate = taxDate;
        }

        public SalesOrderByParameter()
        {
        }

        public string NumberOrder { get; set; }
        public string NameClient { get; set; }
        public string Status { get; set; }
        public DateTime TaxDate { get; set; }
    }

}

public class Reference
{
    public string body { get; set; }
}

public class TrackingUpdate
{
    public string order { get; set; }
    public string tracking { get; set; }
}