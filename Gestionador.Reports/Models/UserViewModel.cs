namespace Gestionador.Reports.Models
{
    public class UserViewModel
    {
        public int? ID { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}