using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionColegios.ViewModels
{
    public class AlumnoFichaViewModel
    {
        // --- Identificación ---
        public int ID { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        public string Rut { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }
        public string NombreCompleto => $"{ApellidoPaterno} {ApellidoMaterno}, {Nombres}";

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        public int? SexoID { get; set; }
        public string Sexo { get; set; }

        [Required(ErrorMessage = "La nacionalidad es obligatoria.")]
        public int? NacionalidadID { get; set; }
        public string Nacionalidad { get; set; }

        // --- Contacto ---
        public string Direccion { get; set; }
        public int? ComunaID { get; set; }
        public string Comuna { get; set; }
        public int? RegionID { get; set; }
        public string Region { get; set; }
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        // --- Salud ---
        public int? SistemaSaludID { get; set; }
        public string SistemaSalud { get; set; }

        // --- Discapacidades (múltiples, con certificado obligatorio) ---
        public List<AlumnoDiscapacidadViewModel> Discapacidades { get; set; } = new List<AlumnoDiscapacidadViewModel>();
        public List<AlumnoDiscapacidadViewModel> DiscapacidadesPendientes { get; set; } = new List<AlumnoDiscapacidadViewModel>();

        /// <summary>Conveniencia para la cabecera: ¿tiene al menos una discapacidad activa?</summary>
        public bool TieneDiscapacidad => Discapacidades != null && Discapacidades.Any();

        public bool TienePIE { get; set; }

        // --- Datos socioeconómicos y culturales ---
        public int? EtniaID { get; set; }
        public string Etnia { get; set; }
        public int? CondicionSocioeconomicaID { get; set; }
        public string CondicionSocioeconomica { get; set; }
        public int? ViveConID { get; set; }
        public string ViveCon { get; set; }

        // --- Traspaso de RUT (extranjeros) ---
        public int? AlumnoOrigenID { get; set; }
        public string RutAnterior { get; set; }
        public bool TieneRutAnterior => !string.IsNullOrEmpty(RutAnterior);

        // Este alumno fue el ORIGEN de un traspaso (su ficha quedó archivada)
        public bool FueTraspasado { get; set; }
        public int? AlumnoDestinoID { get; set; }
        public string RutNuevo { get; set; }

        // --- Retiro ---
        public DateTime? FechaRetiro { get; set; }
        public string CausalRetiro { get; set; }

        // --- Alergias clasificadas (cargadas desde BD para visualización) ---
        public List<AlumnoAlergiaViewModel> Alergias { get; set; } = new List<AlumnoAlergiaViewModel>();

        // --- Alergias pendientes a guardar (enviadas desde el formulario) ---
        public List<AlumnoAlergiaViewModel> AlergiasPendientes { get; set; } = new List<AlumnoAlergiaViewModel>();

        // --- Estado ---
        public int EstadoAlumnoID { get; set; }
        public string EstadoAlumno { get; set; }
        public string FotoUrl { get; set; }

        // --- Matrícula vigente ---
        public int? MatriculaID { get; set; }
        public int? CursoID { get; set; }
        public string Curso { get; set; }
        public int? AnioEscolar { get; set; }
        public bool EsAlumnoNuevo { get; set; }
        public string ProfesorJefe { get; set; }
        public string EmailProfesorJefe { get; set; }

        // --- Resultado de cierre de año (promoción) ---
        public string ResultadoPromocion { get; set; }
        public bool MatriculaCancelada { get; set; }
        public string MotivoNoPromocion { get; set; }
        public string DecretoNoPromocion { get; set; }
        public string GlosaNoPromocion { get; set; }
        public bool TieneResultadoPromocion => !string.IsNullOrEmpty(ResultadoPromocion);

        // --- Apoderados ---
        public List<ApoderadoResumenViewModel> Apoderados { get; set; } = new List<ApoderadoResumenViewModel>();

        // --- Apoderados inline (crear/editar) ---
        public ApoderadoInlineViewModel ApoderadoPadre { get; set; } = new ApoderadoInlineViewModel();
        public ApoderadoInlineViewModel ApoderadoMadre { get; set; } = new ApoderadoInlineViewModel();

        // --- Documentos ---
        public List<DocumentoAlumnoViewModel> Documentos { get; set; } = new List<DocumentoAlumnoViewModel>();

        // --- Historial ---
        public List<LogActividadViewModel> Historial { get; set; } = new List<LogActividadViewModel>();
    }

    public class ApoderadoResumenViewModel
    {
        public int ID { get; set; }               // ApoderadoID
        public int AlumnoApoderadoID { get; set; } // tbl_AlumnoApoderado.ID (para editar/desvincular)
        public string Rut { get; set; }
        public string NombreCompleto { get; set; }
        public string Parentesco { get; set; }
        public bool EsApoderadoTitular { get; set; }
        public bool EsPadre { get; set; }
        public bool EsMadre { get; set; }
        public string Telefono { get; set; }
        public string TelefonoCelular { get; set; }
        public string Email { get; set; }
        public string NivelEducacional { get; set; }
        public string SituacionLaboral { get; set; }
        public string LugarTrabajo { get; set; }
    }

    /// <summary>Datos de un apoderado capturados inline al crear/editar un alumno.</summary>
    public class ApoderadoInlineViewModel
    {
        public int    ApoderadoID       { get; set; }
        public string Rut               { get; set; }
        public string Nombres           { get; set; }
        public string ApellidoPaterno   { get; set; }
        public string ApellidoMaterno   { get; set; }
        public int?   NacionalidadID    { get; set; }
        public int?   NivelEducacionalID { get; set; }
        public int?   SituacionLaboralID { get; set; }
        public string LugarTrabajo      { get; set; }
        public string Direccion         { get; set; }
        public int?   RegionID          { get; set; }
        public int?   ComunaID          { get; set; }
        public string Telefono          { get; set; }
        public string TelefonoCelular   { get; set; }
        public string Email             { get; set; }
        public int    ParentescoID      { get; set; }
        public bool   EsApoderadoTitular { get; set; }
    }

    public class DocumentoAlumnoViewModel
    {
        public int ID { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime FechaCarga { get; set; }
        public bool Obligatorio { get; set; }
    }

    public class AlumnoAlergiaViewModel
    {
        public int ID { get; set; }
        public int AlumnoID { get; set; }

        /// <summary>1 = Informativa | 2 = Alimenticia</summary>
        public int TipoAlergiaID { get; set; }
        public string TipoAlergia { get; set; }

        /// <summary>FK a tbl_CatalogoAlergia (solo para tipo Alimenticia).</summary>
        public int? CatalogoAlergiaID { get; set; }

        /// <summary>
        /// Para Informativa: nombre libre ingresado por el usuario.
        /// Para Alimenticia: nombre del catálogo seleccionado (o "Otros").
        /// </summary>
        public string NombreAlergia { get; set; }

        /// <summary>Obligatorio cuando CatalogoAlergia.RequiereDetalle = true.</summary>
        public string Descripcion { get; set; }

        // Certificado (tipo Informativa)
        public string CertificadoNombre { get; set; }
        public string CertificadoMimeType { get; set; }
        public bool TieneCertificado => !string.IsNullOrEmpty(CertificadoNombre);

        /// <summary>Archivo adjunto enviado en el formulario multipart.</summary>
        public HttpPostedFileBase CertificadoArchivo { get; set; }
    }

    public class AlumnoDiscapacidadViewModel
    {
        public int ID { get; set; }
        public int AlumnoID { get; set; }
        public int TipoDiscapacidadID { get; set; }
        public string TipoDiscapacidad { get; set; }
        public string Descripcion { get; set; }
        public string CertificadoNombre { get; set; }
        public string CertificadoMimeType { get; set; }
        public bool TieneCertificado => !string.IsNullOrEmpty(CertificadoNombre);
        public HttpPostedFileBase CertificadoArchivo { get; set; }
    }

    public class CambioRutViewModel
    {
        public int AlumnoOrigenID { get; set; }
        public string RutNuevo { get; set; }
        public string Motivo { get; set; }
    }

    public class TraspasoRutResumenViewModel
    {
        public int ID { get; set; }
        public string RutAnterior { get; set; }
        public string RutNuevo { get; set; }
        public DateTime FechaTraspaso { get; set; }
        public string Motivo { get; set; }
        public int AlumnoOrigenID { get; set; }
        public int AlumnoDestinoID { get; set; }
    }
}
