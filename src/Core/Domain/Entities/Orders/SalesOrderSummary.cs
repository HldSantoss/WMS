using System;
using System.Text.Json.Serialization;

namespace Domain.Entities.Orders
{
    public class SalesOrderSummary
    {
        public string LimitDate { get; set; }
        public string Status { get; set; }
        public int Qty { get; set; }
    }

    public class SalesOrderSummaryList
    {
        [JsonPropertyName("value")]
        public IEnumerable<SalesOrderSummary> SalesOrders { get; set; }
    }

    public class SalesOrderByUfSummary
    {
        public string LimitDate { get; set; }
        public string Uf { get; set; }
        public int Qty { get; set; }
    }

    public class SalesOrderByUfSummaryList
    {
        [JsonPropertyName("value")]
        public IEnumerable<SalesOrderByUfSummary> SalesOrders { get; set; }
    }

    public class Client
    {
        public string CardCode { get; set; }
    }

    public class ListCardCode
    {
        public List<Client> value { get; set; }
    }

    public class OrdersByCardCode
    {
        public List<OrdersByCode> value { get; set; }
    }

    public class OrdersByCode
    {
        public long DocEntry { get; set; }

        public string CardName { get; set; }

        public string NumAtCard { get; set; }

        public string U_WMS_Status { get; set; }
    }

    public class CardCodeByOrder
    {
        public List<CardCodeByOrderList> value { get; set; }
    }

    public class CardCodeByOrderList
    {
        public string CardCode { get; set; }
    }

}
