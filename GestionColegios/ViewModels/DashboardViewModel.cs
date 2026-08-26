using System;
using System.Collections.Generic;

namespace GestionColegios.ViewModels
{
    public class DashboardViewModel
    {
        // Alumnos
        public int TotalAlumnos { get; set; }
        public int AlumnosVigentes { get; set; }
        public int AlumnosRetirados { get; set; }
        public int AlumnosConPIE { get; set; }

        // Matriculas del anio activo
        public int AnioEscolarActivo { get; set; }
        public int MatriculasVigentes { get; set; }
        public int MatriculasPreMatriculadas { get; set; }
        public int MatriculasAnuladas { get; set; }
        public int MatriculasAlumnosNuevos { get; set; }

        // Documentos
        public int AlumnosConDocumentosPendientes { get; set; }

        // Widgets
        public List<AlumnosPorCursoViewModel> AlumnosPorCurso { get; set; } = new List<AlumnosPorCursoViewModel>();
        public List<ActividadRecienteViewModel> ActividadReciente { get; set; } = new List<ActividadRecienteViewModel>();
    }

    public class AlumnosPorCursoViewModel
    {
        public int CursoID { get; set; }
        public string Curso { get; set; }
        public int Total { get; set; }
        public int Vigentes { get; set; }
    }

    public class ActividadRecienteViewModel
    {
        public string Entidad { get; set; }
        public int EntidadID { get; set; }
        public string Accion { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaAccion { get; set; }
        public string Detalle { get; set; }
    }
}
