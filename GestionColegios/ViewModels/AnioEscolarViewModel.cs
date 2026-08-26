using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionColegios.ViewModels
{
    // ?? Listado de años ???????????????????????????????????????????????????
    public class AnioEscolarListadoViewModel
    {
        public int ID { get; set; }
        public int Anio { get; set; }
        public string Establecimiento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public bool Cerrado { get; set; }
        public bool EsActivo { get; set; }
        public string Estado => EsActivo ? "Activo" : Cerrado ? "Cerrado" : "Inactivo";
        public int TotalCursos { get; set; }
        public int TotalMatriculados { get; set; }
    }

    // ?? Formulario crear / editar año ?????????????????????????????????????
    public class AnioEscolarFormViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2000, 2100, ErrorMessage = "Ingrese un año válido.")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El establecimiento es obligatorio.")]
        public int EstablecimientoID { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public bool Cerrado { get; set; }
        public bool EsActivo { get; set; }

        // Listas de apoyo para los selects
        public List<SelectItemViewModel> Establecimientos { get; set; } = new List<SelectItemViewModel>();
    }

    // ?? Detalle de año (cabecera + cursos) ????????????????????????????????
    public class AnioEscolarDetalleViewModel
    {
        public int ID { get; set; }
        public int Anio { get; set; }
        public string Establecimiento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public bool Cerrado { get; set; }
        public List<CursoDetalleViewModel> Cursos { get; set; } = new List<CursoDetalleViewModel>();
    }

    // ?? Fila de curso en la grilla ????????????????????????????????????????
    public class CursoDetalleViewModel
    {
        public int ID { get; set; }
        public int AnioEscolarID { get; set; }
        public int GradoID { get; set; }
        public string NivelEnsenanza { get; set; }
        public string Grado { get; set; }
        public string Letra { get; set; }
        public string NombreCurso => $"{Grado} {Letra}";
        public int? Capacidad { get; set; }
        public int? ProfesorJefeID { get; set; }
        public string ProfesorJefe { get; set; }
        public int TotalAlumnos { get; set; }
        public bool IsActive { get; set; }
    }

    // ?? Formulario crear / editar curso ???????????????????????????????????
    public class CursoFormViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El año escolar es obligatorio.")]
        public int AnioEscolarID { get; set; }
        public int AnioEscolar { get; set; }

        [Required(ErrorMessage = "El grado es obligatorio.")]
        public int GradoID { get; set; }

        [Required(ErrorMessage = "La letra es obligatoria.")]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "La letra debe tener 1 o 2 caracteres.")]
        public string Letra { get; set; }

        [Range(1, 500, ErrorMessage = "La capacidad debe estar entre 1 y 500.")]
        public int? Capacidad { get; set; }

        public int? ProfesorJefeID { get; set; }

        // Listas de apoyo
        public List<SelectItemViewModel> Grados { get; set; } = new List<SelectItemViewModel>();
        public List<SelectItemViewModel> Profesores { get; set; } = new List<SelectItemViewModel>();
    }

    // ?? Cierre de año: resultado de promoción por matrícula ???????????????
    public class PromocionAlumnoViewModel
    {
        public int MatriculaID { get; set; }
        public int AlumnoID { get; set; }
        public string AlumnoNombreCompleto { get; set; }
        public string AlumnoRut { get; set; }
        public string Curso { get; set; }
        public string NroMatricula { get; set; }
        public string ResultadoPromocion { get; set; }   // "Promovido" | "No Promovido" | null
        public bool MatriculaCancelada { get; set; }
        public string MotivoNoPromocion { get; set; }
        public string DecretoNoPromocion { get; set; }
        public string GlosaNoPromocion { get; set; }
        public DateTime? FechaResultadoPromocion { get; set; }
    }

    /// <summary>
    /// Datos enviados al registrar el resultado de promoción de una matrícula.
    /// </summary>
    public class RegistrarPromocionViewModel
    {
        [Required]
        public int MatriculaID { get; set; }

        /// <summary>"Promovido" o "No Promovido".</summary>
        [Required(ErrorMessage = "Debe indicar el resultado de promoción.")]
        public string ResultadoPromocion { get; set; }

        // Solo aplica cuando ResultadoPromocion == "Promovido"
        public bool MatriculaCancelada { get; set; }

        // Obligatorios cuando ResultadoPromocion == "No Promovido"
        public string MotivoNoPromocion { get; set; }
        public string DecretoNoPromocion { get; set; }
        public string GlosaNoPromocion { get; set; }
    }

    /// <summary>
    /// Solicitud para levantar la condición "Matrícula Cancelada" mediante
    /// autorización con clave de supervisor.
    /// </summary>
    public class AutorizarMatriculaViewModel
    {
        [Required]
        public int MatriculaID { get; set; }

        [Required(ErrorMessage = "Debe ingresar el usuario supervisor.")]
        public string SupervisorUsuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar la clave de supervisor.")]
        public string SupervisorClave { get; set; }

        public string Observacion { get; set; }
    }
}
