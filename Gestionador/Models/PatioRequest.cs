using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class PatioRequest
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un cementerio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cementerio.")]
        public int CementerioID { get; set; }
        [Required(ErrorMessage = "El patio es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre del patio debe tener un máximo de 30 caracteres.")]
        public string Patio { get; set; }
        public bool? IsActive { get; set; }
        public List<SectoresRequest> Sectores { get; set; } = new List<SectoresRequest>();
    }
}