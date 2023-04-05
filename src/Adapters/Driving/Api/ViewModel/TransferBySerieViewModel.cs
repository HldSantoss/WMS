using System.ComponentModel.DataAnnotations;

namespace Api.ViewModel
{
    public class TransferBySerieViewModel : TransferViewModel
    {
        public TransferBySerieViewModel(double quantity, string binCodeFrom, string binCodeTo, string serie)
            : base(quantity, binCodeFrom, binCodeTo)
        {
            Serie = serie;
        }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string Serie { get; init; }
    }
}