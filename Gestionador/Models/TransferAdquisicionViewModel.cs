namespace Gestionador.Models
{
    public class TransferAdquisicionViewModel
    {
        public int ID { get; set; }
        public PersonaViewModel Comprador { get; set; }
        public PersonaViewModel Referente { get; set; }
    }
}