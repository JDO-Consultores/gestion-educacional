using System;

namespace Gestionador.Reports.Responses
{
    public class AdquisicionServiciosResponse
    {
        public int ID { get; set; }
        public int ServicioID { get; set; }
        public int AdquisicionID { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool IsActive { get; set; }
        public ServiciosResponse Servicio { get; set; }

        public string NombreServicio => Servicio.Servicio;
        public string Categoria => Servicio.Categoria;
    }
}