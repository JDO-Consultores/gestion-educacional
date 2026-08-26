using System;

namespace GestionColegios.ViewModels
{
    public class LogActividadViewModel
    {
        public int ID { get; set; }
        public string Entidad { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaAccion { get; set; }
    }
}
