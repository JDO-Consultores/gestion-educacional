using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class CausaRequest
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "El nombre de la causa es obligatorio.")]
        public string Causa{ get; set; }
        public bool IsActive { get; set; }
    }
}