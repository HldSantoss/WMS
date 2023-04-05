using System.ComponentModel.DataAnnotations;

namespace Api.ViewModel
{
    public class TransferByItemCodeViewModel : TransferViewModel
    {
        public TransferByItemCodeViewModel(double quantity, string binCodeFrom, string binCodeTo, string itemCode)
            : base(quantity, binCodeFrom, binCodeTo)
        {
            ItemCode = itemCode;
        }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string ItemCode { get; init; }
    }
}