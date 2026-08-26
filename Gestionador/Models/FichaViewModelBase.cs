namespace Gestionador.Models
{
    public class FichaViewModel : FichaViewModelBase
    {
        public bool IsTransferred { get; set; }
        public int? TransferredID { get; set; }
    }
}