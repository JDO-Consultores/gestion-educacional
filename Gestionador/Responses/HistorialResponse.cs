using System;

namespace Gestionador.Responses
{
    public class HistorialResponse
    {
        public int ID { get; set; }
        public int UsuarioID { get; set; }
        public int AdquisicionID { get; set; }
        public string Accion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public UserResponse User { get; set; }
    }
}