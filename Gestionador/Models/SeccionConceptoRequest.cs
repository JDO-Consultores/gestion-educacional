using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class SeccionConceptoRequest
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "La SECCIÓN es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una SECCIÓN válida")] 
        public int SeccionID { get; set; }
        public int ConceptoID { get; set; }

        [Required(ErrorMessage = "EL STOCK ES REQUERIDO")]
        [Range(0, int.MaxValue, ErrorMessage = "El STOCK NO PUEDE SER NEGATIVO")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "EL TiPO DE MONEDA ES REQUERIDO")]
        [Range(0, int.MaxValue, ErrorMessage = "EL TiPO DE MONEDA ES REQUERIDO")]
        public int TipoMonedaID { get; set; }

        [Required(ErrorMessage = "EL PRECIO DEL SERVICIO ES REQUERIDO")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "El PRECIO NO PUEDE SER NEGATIVO")]
        public decimal Precio { get; set; }
        public bool IsActive { get; set; }
        public List<ConceptoServicioRequest> ServicioRequests { get; set; } = new List<ConceptoServicioRequest>();
    }
}