using System;

namespace GestionColegios.ViewModels
{
    public class AlumnoListadoViewModel
    {
        public int ID { get; set; }
        public string NroMatricula { get; set; }
        public string Rut { get; set; }
        public string NombreCompleto { get; set; }
        public string Curso { get; set; }
        public int? AnioEscolar { get; set; }
        public string EstadoAlumno { get; set; }
        public bool TienePIE { get; set; }
        public string FotoUrl { get; set; }
    }
}