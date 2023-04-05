using System;
namespace Domain.Entities.Orders
{
  public class SalesOrderSummaryByStoreNew
    {
        public List<StatusDash> value { get; set; }
    }

    public class StatusDash
    {
        public int Qty { get; set; }
        public string? Status { get; set; }
        public string? Store { get; set; }
    }
}

