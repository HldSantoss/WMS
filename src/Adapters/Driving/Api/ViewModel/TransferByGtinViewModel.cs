using System.ComponentModel.DataAnnotations;

namespace Api.ViewModel
{
    public class TransferByGtinViewModel : TransferViewModel
    {
        public TransferByGtinViewModel(double quantity, string binCodeFrom, string binCodeTo, string gtin) 
            : base(quantity, binCodeFrom, binCodeTo)
        {
            Gtin = gtin;
        }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string Gtin { get; init; }
    }
}