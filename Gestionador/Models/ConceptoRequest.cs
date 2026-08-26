using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ConceptoRequest
    {
        public int? ID { get; set; }
        public int CategoriaID { get; set; }
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        public string Concepto { get; set; }
        public bool IsNicho { get; set; }
        public bool IsActive { get; set; }
    }
}