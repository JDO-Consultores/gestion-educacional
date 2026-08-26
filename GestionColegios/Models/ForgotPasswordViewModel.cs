using System.ComponentModel.DataAnnotations;

namespace GestionColegios.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }
    }
}