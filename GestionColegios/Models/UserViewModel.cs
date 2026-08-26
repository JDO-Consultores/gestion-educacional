using System.ComponentModel.DataAnnotations;

namespace GestionColegios.Models
{
    public class UserViewModel
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "El correo (username) es obligatorio.")]
        [EmailAddress(ErrorMessage = "El username debe ser un correo válido.")]
        public string Username { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        // Opcional en edición: si se deja en blanco, se mantiene la contraseña actual.
        [RegularExpression(@"^(?=.{8,16}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una letra mayúscula y un carácter especial.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}