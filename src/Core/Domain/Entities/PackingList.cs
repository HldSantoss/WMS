using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class PackingList
    {
        public PackingList()
        {

        }

        public PackingList(long? docNum, int period, int instance, int series, string handwrtten, string status, string requestStatus, string creator, object remark, int docEntry, string canceled, string @object, object logInst, int userSign, string transfered, DateTime createDate, TimeSpan createTime, DateTime updateDate, TimeSpan updateTime, DateTime? closedDateAt, TimeSpan? closedTimeAt, DateTime? exportedDateAt, DateTime? exportedTimeAt, string carrierId, string method, int branch, List<PackingListItem>? items)
        {
            DocNum = docNum;
            Period = period;
            Instance = instance;
            Series = series;
            Handwrtten = handwrtten;
            Status = status;
            RequestStatus = requestStatus;
            Creator = creator;
            Remark = remark;
            DocEntry = docEntry;
            Canceled = canceled;
            Object = @object;
            LogInst = logInst;
            UserSign = userSign;
            Transfered = transfered;
            CreateDate = createDate;
            CreateTime = createTime;
            UpdateDate = updateDate;
            UpdateTime = updateTime;
            ClosedDateAt = closedDateAt;
            ClosedTimeAt = closedTimeAt;
            ExportedDateAt = exportedDateAt;
            ExportedTimeAt = exportedTimeAt;
            CarrierId = carrierId;
            Method = method;
            Branch = branch;
            Items = items;
        }

        public long? DocNum { get; set; }
        public int Period { get; set; }
        public int Instance { get; set; }
        public int Series { get; set; }
        public string Handwrtten { get; set; }
        public string Status { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public object Remark { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int UserSign { get; set; }
        public string Transfered { get; set; }
        public DateTime CreateDate { get; set; }
        public TimeSpan CreateTime { get; set; }
        public DateTime UpdateDate { get; set; }
        public TimeSpan UpdateTime { get; set; }

        [JsonPropertyName("U_DateClosedAt")]
        public DateTime? ClosedDateAt { get; set; }

        [JsonPropertyName("U_TimeClosedAt")]
        public TimeSpan? ClosedTimeAt { get; set; }

        [JsonPropertyName("U_DateExportedAt")]
        public DateTime? ExportedDateAt { get; set; }

        [JsonPropertyName("U_TimeExportedAt")]
        public DateTime? ExportedTimeAt { get; set; }

        [JsonPropertyName("U_CarrierId")]
        public string CarrierId { get; set; }

        [JsonPropertyName("U_CarrierName")]
        public string CarrierName { get; set; }

        [JsonPropertyName("U_Method")]
        public string Method { get; set; }

        [JsonPropertyName("U_BPLId")]
        public int Branch { get; set; }

        [JsonPropertyName("PACKINGLISTITEMCollection")]
        public List<PackingListItem>? Items { get; set; }

        public bool IsCanCancelled()
        {
            return (Items == null || !Items.Any());
        }
    }

    public class PackingListItem
    {
        public PackingListItem(long lineNum, DateTime createDate, TimeSpan createTime, long invoiceEntry, string keyNfe)
        {
            LineNum = lineNum;
            CreateDate = createDate;
            CreateTime = createTime;
            InvoiceEntry = invoiceEntry;
            KeyNfe = keyNfe;
        }

        public PackingListItem()
        {
        }

        [JsonPropertyName("LineId")]
        public long LineNum { get; set; }

        [JsonPropertyName("U_CreateDate")]
        public DateTime CreateDate { get; set; }

        [JsonPropertyName("U_CreateTime")]
        public TimeSpan CreateTime { get; set; }

        [JsonPropertyName("U_InvoiceEntry")]
        public long InvoiceEntry { get; set; }

        [JsonPropertyName("U_KeyNfe")]
        public string KeyNfe { get; set; }

        [JsonPropertyName("U_CardName")]
        public string CardName { get; set; }

        [JsonPropertyName("U_DocNum")]
        public int? DocNum { get; set; }

        [JsonPropertyName("U_SequenceSerial")]
        public long SequenceSerial { get; set; }

        [JsonPropertyName("U_SeriesStr")]
        public string SeriesStr { get; set; }
    }

    public class PackingListRoot
    {
        [JsonPropertyName("value")]
        public List<PackingListItemRoot?> Packinglists { get; set; }
    }

    public class PackingListItemRoot
    {
        public int DocNum { get; set; }
        public int Period { get; set; }
        public int Instance { get; set; }
        public int Series { get; set; }
        public string Handwrtten { get; set; }
        public string Status { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public object Remark { get; set; }
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
        public object U_DateClosedAt { get; set; }
        public object U_TimeClosedAt { get; set; }
        public object U_DateExportedAt { get; set; }
        public object U_TimeExportedAt { get; set; }
        public string U_Method { get; set; }
        public string U_CarrierId { get; set; }
        public int U_BPLId { get; set; }
        public object U_CarrierName { get; set; }
        public List<PACKINGLISTITEMCollectionRoot> PACKINGLISTITEMCollection { get; set; }
    }

    public class PACKINGLISTITEMCollectionRoot
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public int VisOrder { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public int U_InvoiceEntry { get; set; }
        public string U_CreateDate { get; set; }
        public string U_CreateTime { get; set; }
        public string U_KeyNfe { get; set; }
        public string U_CardName { get; set; }
        public int? U_DocNum { get; set; }
        public int? U_SequenceSerial { get; set; }
        public string U_SeriesStr { get; set; }
    }

    public class PackingListUpsert
    {
        public PackingListUpsert()
        {
        }

        public PackingListUpsert(List<PackingListUpsertItem>? items)
        {
            Items = items;
        }

        [JsonPropertyName("PACKINGLISTITEMCollection")]
        public List<PackingListUpsertItem> Items { get; set; }
    }

    public class PackingListUpsertItem
    {
        public PackingListUpsertItem(string createDate,
                                     string createTime,
                                     long invoiceEntry,
                                     string keyNfe,
                                     string cardName,
                                     long? docNum,
                                     long sequenceSerial,
                                     string seriesStr)
        {
            CreateDate = createDate;
            CreateTime = createTime;
            InvoiceEntry = invoiceEntry;
            KeyNfe = keyNfe;
            CardName = cardName;
            DocNum = docNum;
            SequenceSerial = sequenceSerial;
            SeriesStr = seriesStr;
        }

        [JsonPropertyName("U_CreateDate")]
        public string CreateDate { get; set; }

        [JsonPropertyName("U_CreateTime")]
        public string CreateTime { get; set; }

        [JsonPropertyName("U_InvoiceEntry")]
        public long InvoiceEntry { get; set; }

        [JsonPropertyName("U_KeyNfe")]
        public string KeyNfe { get; set; }

        [JsonPropertyName("U_CardName")]
        public string CardName { get; set; }

        [JsonPropertyName("U_DocNum")]
        public long? DocNum { get; set; }

        [JsonPropertyName("U_SequenceSerial")]
        public long SequenceSerial { get; set; }

        [JsonPropertyName("U_SeriesStr")]
        public string SeriesStr { get; set; }
    }

    public class TrackingVtex
    {
        public string trackingNumber { get; set; }
        public string trackingUrl { get; set; }
        public string courier { get; set; }
        public DateTime dispatchedDate { get; set; }
        public string orderId { get; set; }
        public string invoiceNumber { get; set; }
    }
}