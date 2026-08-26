using System;
using System.Collections.Generic;
using System.Web;

namespace GestionColegios.ViewModels
{
    /// <summary>
    /// Vista de un documento existente del alumno.
    /// </summary>
    public class DocumentoAlumnoDetalleViewModel
    {
        public int ID { get; set; }
        public int AlumnoID { get; set; }
        public int TipoDocumentoID { get; set; }
        public string TipoDocumento { get; set; }
        public string Categoria { get; set; }
        public bool Obligatorio { get; set; }
        public bool EsAnexo { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime FechaCarga { get; set; }
        public string Observacion { get; set; }
        public int? AnioEscolarID { get; set; }
        public int? AnioEscolar { get; set; }
    }

    /// <summary>
    /// Estado de un tipo de documento de matrícula para un año determinado.
    /// Indica si ya fue cargado o está pendiente.
    /// </summary>
    public class DocumentoMatriculaEstadoViewModel
    {
        public int TipoDocumentoID { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public bool Obligatorio { get; set; }
        public bool Cargado { get; set; }
        public int? DocumentoAlumnoID { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime? FechaCarga { get; set; }
        /// <summary>1=Oblig.Pendiente 2=Oblig.Cargado 3=Opc.Pendiente 4=Opc.Cargado</summary>
        public int OrdenImportancia { get; set; }
        public int OrdenCategoria { get; set; }
    }

    /// <summary>
    /// Resultado de la verificación de documentos obligatorios para una matrícula.
    /// </summary>
    public class VerificacionDocumentosResult
    {
        public bool TodosObligatoriosCargados { get; set; }
        public List<string> FaltanDocumentos { get; set; } = new List<string>();
    }

    /// <summary>
    /// ViewModel para subir un documento al alumno.
    /// </summary>
    public class SubirDocumentoViewModel
    {
        public int AlumnoID { get; set; }
        public int TipoDocumentoID { get; set; }
        public int? AnioEscolarID { get; set; }
        public string Observacion { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
    }

    /// <summary>
    /// ViewModel para la vista del Tab Documentos en la ficha del alumno.
    /// Agrupa documentos de matrícula por año y documentos anexos.
    /// </summary>
    public class DocumentosFichaViewModel
    {
        public int AlumnoID { get; set; }

        /// <summary>
        /// Años escolares disponibles para filtrar documentos de matrícula.
        /// </summary>
        public List<SelectItemViewModel> AniosEscolares { get; set; } = new List<SelectItemViewModel>();

        /// <summary>
        /// Todos los tipos de documento de matrícula (EsAnexo=false), agrupados por categoría,
        /// con su estado de carga para el año seleccionado.
        /// </summary>
        public List<DocumentoMatriculaEstadoViewModel> DocumentosMatricula { get; set; } = new List<DocumentoMatriculaEstadoViewModel>();

        /// <summary>
        /// Documentos anexos del alumno (EsAnexo=true), no ligados a un año.
        /// </summary>
        public List<DocumentoAlumnoDetalleViewModel> DocumentosAnexos { get; set; } = new List<DocumentoAlumnoDetalleViewModel>();

        /// <summary>
        /// Tipos de documento disponibles para subir como anexo.
        /// </summary>
        public List<SelectItemViewModel> TiposDocumentoAnexo { get; set; } = new List<SelectItemViewModel>();

        /// <summary>
        /// ¿Están todos los documentos obligatorios cargados para el año actual?
        /// </summary>
        public bool TodosObligatoriosCargados { get; set; }
        public List<string> DocumentosFaltantes { get; set; } = new List<string>();
    }
}
