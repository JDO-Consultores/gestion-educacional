using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class LugarDefuncionRequest
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "El nombre del lugar es obligatorio.")]
        public string Lugar { get; set; }
        public bool IsActive { get; set; }
    }
}