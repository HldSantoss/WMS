using System.ComponentModel.DataAnnotations;

namespace Api.ViewModel
{
    public class PackingListCreateViewModel
    {
        public PackingListCreateViewModel(string carrierId, string method, int branch)
        {
            CarrierId = carrierId;
            Method = method;
            Branch = branch;
        }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string CarrierId { get; set; }
        public string? CarrierName { get; set; }

        [Required(ErrorMessage = "{0} é um campo obrigatório")]
        public string Method { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} é um campo obrigatório")]
        public int Branch { get; set; }
    }
}