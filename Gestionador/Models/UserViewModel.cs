using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class UserViewModel
    {
        public int? ID { get; set; }
        [EmailAddress(ErrorMessage = "Ingrese un email valido.")]
        public string Username { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}