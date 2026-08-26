using System.ComponentModel.DataAnnotations;

namespace GestionColegios.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}