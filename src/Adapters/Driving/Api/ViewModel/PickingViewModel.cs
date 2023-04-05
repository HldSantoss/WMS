using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Validations;

namespace Api.ViewModel
{
    public record PickingViewModel
    {
        public long DocEntry { get; init; }
        public long? DocNum { get; init; }
        public string? NumAtCard { get; init; }
        public string? CardName { get; init; }        
        public string? CarrierMethod { get; set; }
        public string? CarrierType { get; set; }
        public string? TrackingCode { get; set; }
        public string? Carrier { get; init; }
        public string? CarrierName { get; set; }
        public int BPLId { get; set; }
        public List<LabelViewModel>? Labels { get; set; }

        public IEnumerable<PickingItemViewModel> Items { get; init; }
    }

    public record PickingItemViewModel
    {
        public int LineNum { get; init; }

        public string ItemCode { get; init; }
        public string? BarCode { get; set; }
        public string? ItemDescription { get; init; }
        public string? ManSerNum { get; set; }

        public string ManBtchNum { get; set; }

        public double Quantity { get; init; }

        public double QuantityPicked { get; init; }

        public IEnumerable<BinLocationsViewModel> BinAllocations { get; init; }

        public IEnumerable<SeriesViewModel>? SerialNumbers { get; init; }
        public IEnumerable<LabelViewModel>? Labels { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ManSerNum == "Y" && (SerialNumbers == null || !SerialNumbers.Any()))
            {
                yield return new ValidationResult(
                    $"Itens controlados por série precisam do SerialNumbers preenchido.",
                    new[] { nameof(SerialNumbers) });
            }
        }

        [JsonIgnore]
        public bool IsFinish 
        {
            get { return !BinAllocations.Where(p => p.IsFinish == false).Any(); }
        }
    }

    public record BinLocationsViewModel
    {
        public long BaseLineNumber { get; init; }

        public long SerialAndBatchNumbersBaseLine { get; init; }
        
        public long BinAbs { get; set; }
        public string? BinCode { get; init; }
        
        public double Quantity { get; init; }
        public double PickedQuantity { get; init; }
        public bool IsFinish 
        {
            get { return Quantity == PickedQuantity; }
        }
    }

    public record SeriesViewModel
    {
        public long LineNum { get; init; }
        public long BaseLineNumber { get; init; }
        public string InternalSerialNumber { get; init; }
        public double Quantity { get; set; }

    }

    public record LabelViewModel
    {
        public string Zpl { get; init; }
    }
}