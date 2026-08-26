using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionColegios.ViewModels
{
    public class MatriculaViewModel
    {
        public int ID { get; set; }

        [Required]
        public int AlumnoID { get; set; }
        public string AlumnoNombreCompleto { get; set; }
        public string AlumnoRut { get; set; }

        [Required]
        public int CursoID { get; set; }
        public string CursoNombre { get; set; }

        [Required]
        public int AnioEscolarID { get; set; }
        public int AnioEscolar { get; set; }

        public string NroMatricula { get; set; }

        // N?mero de matr?cula anterior (cuando el alumno se retir? y reingres?).
        // Se conserva como hist?rico y permite relacionar la matr?cula actual con la previa.
        public string NroMatriculaAnterior { get; set; }

        [Required]
        public DateTime FechaMatricula { get; set; }

        [Required]
        public int EstadoMatriculaID { get; set; }
        public string EstadoMatricula { get; set; }

        public bool EsAlumnoNuevo { get; set; }
        public string TipoAlumno => EsAlumnoNuevo ? "Alumno Nuevo" : "Alumno Antiguo";

        // Indica que la matr?cula qued? en lista de espera por falta de cupos en el curso.
        public bool EnListaEspera { get; set; }

        public string Observacion { get; set; }

        // Auditoría (solo lectura en vista)
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }

    /// <summary>
    /// Datos para poblar los dropdowns del formulario de matrícula.
    /// </summary>
    public class MatriculaFormViewModel
    {
        public int AlumnoID { get; set; }
        public string AlumnoNombreCompleto { get; set; }
        public string AlumnoRut { get; set; }
        public bool EsAlumnoNuevo { get; set; }

        public List<SelectItemViewModel> AniosEscolares { get; set; }
        public List<SelectItemViewModel> Cursos { get; set; }
        public List<SelectItemViewModel> EstadosMatricula { get; set; }

        // Matrícula a editar (null = nueva)
        public MatriculaViewModel Matricula { get; set; }
    }

    public class SelectItemViewModel
    {
        public int ID { get; set; }
        public string Texto { get; set; }        
    }

    /// <summary>
    /// Resultado de crear una matr?cula. Permite informar al controlador si la
    /// matr?cula qued? en lista de espera y relacionar el n?mero anterior (reingreso).
    /// </summary>
    public class MatriculaResultado
    {
        /// <summary>ID de la matr?cula creada (&gt;0), 0 = alumno inexistente, -1 = duplicada en el a?o, -3 = matr?cula cancelada (bloqueado).</summary>
        public int MatriculaID { get; set; }
        public bool EnListaEspera { get; set; }
        public string NroMatricula { get; set; }
        public string NroMatriculaAnterior { get; set; }
    }

    /// <summary>
    /// Resumen estadístico de matrículas para el encabezado de la vista Index.
    /// </summary>
    public class MatriculaResumenAnioViewModel
    {
        public int AnioEscolarID { get; set; }
        public int Anio { get; set; }
        public int TotalMatriculas { get; set; }
        public int Vigentes { get; set; }
        public int PreMatriculados { get; set; }
        public int Anuladas { get; set; }
        public int EnListaEspera { get; set; }
        public int AlumnosNuevos { get; set; }
        public int AlumnosAntiguos { get; set; }
        public List<SelectItemViewModel> AniosDisponibles { get; set; } = new List<SelectItemViewModel>();
    }
}