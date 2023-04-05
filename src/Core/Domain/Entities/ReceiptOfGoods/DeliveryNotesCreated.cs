using System;
namespace Domain.Entities.ReceiptOfGoods
{
    public class DeliveryNotesCreated
    {
        public DeliveryNotesCreated(int bPL_IDAssignedToInvoice, string docDate, string docDueDate, string numAtCard, int sequenceCode, long sequenceSerial, string seriesString, string subSeriesString, string sequenceModel, List<DocumentLine> documentLines)
        {
            BPL_IDAssignedToInvoice = bPL_IDAssignedToInvoice;
            DocDate = docDate;
            DocDueDate = docDueDate;
            NumAtCard = numAtCard;
            SequenceCode = sequenceCode;
            SequenceSerial = sequenceSerial;
            SeriesString = seriesString;
            SubSeriesString = subSeriesString;
            SequenceModel = sequenceModel;
            DocumentLines = documentLines;
        }

        public DeliveryNotesCreated()
        {
        }

        public int BPL_IDAssignedToInvoice { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string NumAtCard { get; set; }
        public int SequenceCode { get; set; }
        public long SequenceSerial { get; set; }
        public string SeriesString { get; set; }
        public string SubSeriesString { get; set; }
        public string SequenceModel { get; set; }
        public List<DocumentLine> DocumentLines { get; set; }
    }

    public class DocumentLine
    {
        public DocumentLine(string itemCode, double quantity, int baseType, int baseEntry, int baseLine, List<SerialNumber> serialNumbers)
        {
            ItemCode = itemCode;
            Quantity = quantity;
            BaseType = baseType;
            BaseEntry = baseEntry;
            BaseLine = baseLine;
            SerialNumbers = serialNumbers;
        }

        public DocumentLine()
        {
        }

        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public List<SerialNumber> SerialNumbers { get; set; }
    }

    public class SerialNumber
    {
        public SerialNumber(string internalSerialNumber, int baseLineNumber, double quantity)
        {
            InternalSerialNumber = internalSerialNumber;
            BaseLineNumber = baseLineNumber;
            Quantity = quantity;
        }
        public SerialNumber()
        {
        }

        public string InternalSerialNumber { get; set; }
        public int BaseLineNumber { get; set; }
        public double Quantity { get; set; }
    }
}

