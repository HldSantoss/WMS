using System;
namespace Domain.Entities.Orders
{
    
    public class DocumentLine
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public string ShipDate { get; set; }
        public double Price { get; set; }
        public object SerialNum { get; set; }
        public string WarehouseCode { get; set; }
        public int DocEntry { get; set; }
     }

    public class OrderSAP
    {
        public List<OrderDocument> value { get; set; }
    }

    public class OrderDocument
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string NumAtCard { get; set; }
        public double DocTotal { get; set; }
        public string Comments { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; }
        public string ShipToCode { get; set; }
        public string CancelDate { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public object TrackingNumber { get; set; }
        public object SequenceCode { get; set; }
        public object SequenceSerial { get; set; }
        public object SeriesString { get; set; }
        public object SubSeriesString { get; set; }
        public string SequenceModel { get; set; }
        public string Cancelled { get; set; }
        public string BPLName { get; set; }
        public object Supplier { get; set; }
        public string ShipFrom { get; set; }
        public string U_AS_NUMPED { get; set; }
        public object U_AS_CANCELADO_VTEX { get; set; }
        public object U_CT_LoginWms { get; set; }
        public string U_WMS_Status { get; set; }
        public object U_SMW_LoginWMS { get; set; }
        public string U_PickingGroup { get; set; }
        public object U_CT_Store { get; set; }
        public object U_CT_Qualidade { get; set; }
        public string U_CT_PackingListId { get; set; }
        public string U_CT_ReturnSefaz { get; set; }
        public string U_CT_TrackingCode { get; set; }
        public string U_CT_Method { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }
        public TaxExtension TaxExtension { get; set; }
        public AddressExtension AddressExtension { get; set; }
    }

    public class TaxExtension
    {
        public string TaxId0 { get; set; }
        public string TaxId1 { get; set; }
        public object TaxId2 { get; set; }
        public object TaxId3 { get; set; }
        public string TaxId4 { get; set; }
        public object TaxId5 { get; set; }
        public object TaxId6 { get; set; }
        public object TaxId7 { get; set; }
        public object TaxId8 { get; set; }
        public object TaxId9 { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Incoterms { get; set; }
        public object Vehicle { get; set; }
        public object VehicleState { get; set; }
        public object NFRef { get; set; }
        public string Carrier { get; set; }
        public object PackQuantity { get; set; }
        public object PackDescription { get; set; }
        public object Brand { get; set; }
        public object ShipUnitNo { get; set; }
        public double NetWeight { get; set; }
        public double GrossWeight { get; set; }
        public string StreetS { get; set; }
        public string BlockS { get; set; }
        public string BuildingS { get; set; }
        public string CityS { get; set; }
        public string ZipCodeS { get; set; }
        public string CountyS { get; set; }
        public string StateS { get; set; }
        public string CountryS { get; set; }
        public string StreetB { get; set; }
        public string BlockB { get; set; }
        public string BuildingB { get; set; }
        public string CityB { get; set; }
        public string ZipCodeB { get; set; }
        public string CountyB { get; set; }
        public string StateB { get; set; }
        public string CountryB { get; set; }
        public object ImportOrExport { get; set; }
        public object GlobalLocationNumberS { get; set; }
        public object GlobalLocationNumberB { get; set; }
        public string TaxId12 { get; set; }
        public object TaxId13 { get; set; }
        public object BillOfEntryNo { get; set; }
        public object BillOfEntryDate { get; set; }
        public object OriginalBillOfEntryNo { get; set; }
        public object OriginalBillOfEntryDate { get; set; }
        public string ImportOrExportType { get; set; }
        public object PortCode { get; set; }
        public int DocEntry { get; set; }
        public double BoEValue { get; set; }
        public object ClaimRefund { get; set; }
        public object DifferentialOfTaxRate { get; set; }
        public object IsIGSTAccount { get; set; }
    }

}

