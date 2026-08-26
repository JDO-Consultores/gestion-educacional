using System;

namespace Gestionador.Responses
{
    public class AdquisicionMantencionResponse
    {
        public int ID { get; set; }
        public int MantencionID { get; set; }
        public decimal Precio { get; set; }
        public bool IsActive { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioID{ get; set; }
        public int AdquisicionID { get; set; }
        public int Anio { get; set; }
        public string Observacion { get; set; }
        public MantencionResponse Mantencion { get; set; }

        public string NombreMantencion => Mantencion.Mantencion;
        public string Categoria => Mantencion.Categoria;
    }
}