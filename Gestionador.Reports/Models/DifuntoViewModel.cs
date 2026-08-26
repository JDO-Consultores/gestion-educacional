using System;

namespace Gestionador.Reports.Models
{
    public class DifuntoViewModel
    {
        public int ID { get; set; }
        public string NombreApellido { get; set; }
        public string Rut { get; set; }
        public int Edad { get; set; }
        public DateTime FechaDefuncion { get; set; }
        public int CausaID { get; set; }
        public string Causa { get; set; }
        public int LugarID { get; set; }
        public string Lugar { get; set; }
        public bool? Reuso { get; set; }
        public DateTime? FechaReuso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool IsActive { get; set; }
    }
}