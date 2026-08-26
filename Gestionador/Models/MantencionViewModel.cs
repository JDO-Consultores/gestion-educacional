namespace Gestionador.Models
{
    public class MantencionViewModel
    {
        public int ID { get; set; }
        public int MantencionID { get; set; }
        public int Anio { get; set; }
        public decimal Precio { get; set; }
        public string Observacion { get; set; }
    }
}