using System;
namespace Domain.Entities
{
	

    public class StockVirtual
    {
        public List<StockVirtualLine> value { get; set; }
    }

    public class StockVirtualLine
    {
        public string Code { get; set; }
        public object Name { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_ItemCode { get; set; }
        public string U_ItemDescription { get; set; }
        public int? U_Quantity { get; set; }
        public string U_InvoiceImportation { get; set; }
        public string U_Location { get; set; }
        public string U_GroupItem { get; set; }
    }

    public class UpdateObj
    {
        public int Code { get; set; }
        public string U_ItemCode { get; set; }
        public string U_ItemDescription { get; set; }
        public string U_InvoiceImportation { get; set; }
        public string U_Location { get; set; }
        public string U_GroupItem { get; set; }
        public int U_Quantity { get; set; }
    }

}

