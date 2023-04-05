using System;
namespace Domain.Entities.Orders
{
	

    public class OpenOrdersMarketPlaceList
    {
        public List<OpenOrdersMarketPlace>? value { get; set; }
    }

    public class OpenOrdersMarketPlace
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string? CardName { get; set; }
        public string? NumAtCard { get; set; }
        public string? Comments { get; set; }
        public string? CreationDate { get; set; }
        public string? DocumentStatus { get; set; }
        public string? Cancelled { get; set; }
        public string? U_AS_NUMPED { get; set; }
        public string? U_WMS_Status { get; set; }
        public string? U_PickingGroup { get; set; }
        public string? U_CT_Label { get; set; }
        public string? U_CT_PackingListId { get; set; }
        public string? UpdateDate { get; set; }
        public string? UpdateTime { get; set; }
    }

    public class InfoDetails
    {
        public List<Info?> value { get; set; }
    }

    public class Info
    {
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public string? Cellular { get; set; }
        public long? DocNum { get; set; }
        public string? E_Mail { get; set; }
        public string? Email { get; set; }
        public string? Phone1 { get; set; }
        public long? Serial { get; set; }
        public string? SeriesStr { get; set; }
        public int? U_CT_Intelipost { get; set; }
        public string? U_ChaveAcesso { get; set; }
        public string? U_TX_DtComp { get; set; }
    }
}

