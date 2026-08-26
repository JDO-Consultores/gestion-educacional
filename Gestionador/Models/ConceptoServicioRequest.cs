using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ConceptoServicioRequest
    {
        public int? ID { get; set; }
        public int ServicioID { get; set; }
        public int ConceptoID { get; set; }
        public int SeccionID { get; set; }
        public int TipoMonedaID { get; set; }

        [Required(ErrorMessage = "EL PRECIO DEL SERVICIO ES REQUERIDO")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "El PRECIO NO PUEDE SER NEGATIVO")]
        public decimal Precio { get; set; }
    }
}