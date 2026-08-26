namespace Gestionador.Reports.Models
{
    public class UserCreateViewModel : UserViewModel
    {
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}