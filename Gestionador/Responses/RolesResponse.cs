namespace Gestionador.Responses
{
    public class RolesResponse
    {
        public int ID { get; set; }
        public string Rol { get; set; }
        public RolesUsuarios Roles { get; set; }
    }
}