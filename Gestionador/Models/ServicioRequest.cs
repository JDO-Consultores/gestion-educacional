using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ServicioRequest
    {
        public int? ID  { get; set; }
        [Required]
        public int CategoriaID { get; set; }
        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        public string Servicio { get; set; }
        public bool IsActive { get; set; }
    }
}