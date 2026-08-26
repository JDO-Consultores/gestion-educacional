namespace Gestionador.Responses
{
    public class RolesUsuarios
    {
        public int ID { get; set; }
        public int RolID { get; set; }
        public int UsuarioID { get; set; }
        public RolesResponse Roles { get; set; }
        public UserResponse Usuario { get; set; }
    }
}