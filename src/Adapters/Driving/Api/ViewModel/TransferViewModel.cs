using System.ComponentModel.DataAnnotations;
using Api.Validations;

namespace Api.ViewModel
{
    public class TransferViewModel
    {
        public TransferViewModel(double quantity, string binCodeFrom, string binCodeTo)
        {
            Quantity = quantity;
            BinCodeFrom = binCodeFrom;
            BinCodeTo = binCodeTo;
        }

        [QuantityAttribute(ErrorMessage = "{0} é um campo obrigatório")]
        public double Quantity { get; init; }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string BinCodeFrom { get; init; }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string BinCodeTo { get; init; }
    }
}