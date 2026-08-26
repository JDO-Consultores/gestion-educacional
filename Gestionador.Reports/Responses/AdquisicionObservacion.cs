using System;

namespace Gestionador.Reports.Responses
{
    public class AdquisicionObservacion
    {
        public int ID { get; set; }
        public int UsuarioID { get; set; }
        public string Observacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public UserResponse User { get; set; }
    }
}