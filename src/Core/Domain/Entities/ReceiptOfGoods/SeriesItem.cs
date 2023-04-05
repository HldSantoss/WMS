using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods
{
    public class SeriesItem
    {
        public SeriesItem(string? code, string? name, string? uTimestamp, long? docEntry, long? docType, List<SeriesItemLine> items)
        {
            Code = code;
            Name = name;
            UTimestamp = uTimestamp;
            DocEntry = docEntry;
            DocType = docType;
            Items = items;
        }

        public SeriesItem()
        {

        }

        public string? Code { get; set; }

        public string? Name { get; set; }

        [JsonPropertyName("U_TimeStamp")]
        public string? UTimestamp { get; set; }

        [JsonPropertyName("U_DocEntry")]
        public long? DocEntry { get; set; }

        [JsonPropertyName("U_DocType")]
        public long? DocType { get; set; }

        [JsonPropertyName("CT_SERIESITEMSLINHACollection")]
        public List<SeriesItemLine> Items { get; set; }
    }

    public class SeriesItemLine
    {
        public SeriesItemLine(int line, string dateTesting, string userWms, string? serie, string isDefective, string patrimony, string confirmed, string materialCode, string seriesControlled)
        {
            Line = line;
            DateTesting = dateTesting;
            UserWms = userWms;
            Serie = serie;
            IsDefective = isDefective;
            Patrimony = patrimony;
            Confirmed = confirmed;
            MaterialCode = materialCode;
            SeriesControlled = seriesControlled;
        }

        public SeriesItemLine()
        {

        }

        public SeriesItemLine(string dateTesting, string userWms, string? serie, string patrimony, string confirmed, string materialCode)
        {
            DateTesting = dateTesting;
            UserWms = userWms;
            Serie = serie;
            Patrimony = patrimony;
            Confirmed = confirmed;
            MaterialCode = materialCode;
        }


        [JsonPropertyName("LineId")]
        public int Line { get; set; }

        [JsonPropertyName("U_TimeStamp")]
        public string DateTesting { get; set; }

        [JsonPropertyName("U_UserSing")]
        public string UserWms { get; set; }

        [JsonPropertyName("U_Series")]
        public string? Serie { get; set; }

        [JsonPropertyName("U_Number")]
        public string? MaterialCode { get; set; }

        [JsonPropertyName("U_IsFailure")]
        public string IsDefective { get; set; }

        [JsonPropertyName("U_CT_Patrimony")]
        public string Patrimony { get; set; }

        [JsonPropertyName("U_Confirmed")]
        public string Confirmed { get; set; }

        [JsonPropertyName("U_LineOrder")]
        public string SeriesControlled { get; set; }
    }

    public class SeriesItemHeader
    {
        public SeriesItemHeader(List<SeriesItemLine> items)
        {
            Items = items;
        }

        public SeriesItemHeader()
        {

        }

        [JsonPropertyName("CT_SERIESITEMSLINHACollection")]
        public List<SeriesItemLine> Items { get; set; }
    }

    public class ReceivingItemBySerie
    {
        public ReceivingItemBySerie(string serie, string user, string materialCode)
        {
            Serie = serie;
            User = user;
            MaterialCode = materialCode;
        }

        public ReceivingItemBySerie()
        {

        }

        public string Serie { get; set; }
        public string User { get; set; }
        public string MaterialCode { get; set; }

    }

}