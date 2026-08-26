using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "El token es inválido o expiró")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [RegularExpression(@"^(?=.{8,16}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula y un carácter especial.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}