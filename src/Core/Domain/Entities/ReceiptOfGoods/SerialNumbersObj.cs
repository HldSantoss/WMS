using System;
using System.Text.Json.Serialization;

namespace Domain.Entities.ReceiptOfGoods
{
    public class SerialNumbersObj
    {
        [JsonPropertyName("value")]
        public List<SerialNumbersObjItem> Value { get; set; }
    }

    public class SerialNumbersObjItem
    {
        public long DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
    }

    public class UpdateSerialNumber
    {
        public UpdateSerialNumber(string mfrSerialNo)
        {
            MfrSerialNo = mfrSerialNo;
        }

        public string MfrSerialNo { get; set; }
    }

}

