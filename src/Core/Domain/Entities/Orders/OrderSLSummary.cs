using System;
namespace Domain.Entities.Orders
{
    public class OrderSLSummary
    {
         public List<OrderSL>? value { get; set; }
    }

    public class OrderSL
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? DocDate { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? NumAtCard { get; set; }
        public double? DocTotal { get; set; }
        public string? Comments { get; set; }
        public string? CreationDate { get; set; }
        public string? U_AS_NUMPED { get; set; }
        public string? U_CT_LoginWms { get; set; }
        public string? U_WMS_Status { get; set; }
        public string? U_PickingGroup { get; set; }
        public string? U_CT_Store { get; set; }
    }
}

