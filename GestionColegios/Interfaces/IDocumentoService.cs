using GestionColegios.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace GestionColegios.Interfaces
{
    public interface IDocumentoService
    {
        /// <summary>
        /// Obtiene el estado de todos los documentos de matrícula (EsAnexo=false)
        /// para un alumno en un año escolar determinado.
        /// </summary>
        Task<List<DocumentoMatriculaEstadoViewModel>> GetEstadoDocumentosMatriculaAsync(int alumnoId, int anioEscolarId);

        /// <summary>
        /// Obtiene los documentos anexos del alumno (EsAnexo=true, sin año escolar).
        /// </summary>
        Task<List<DocumentoAlumnoDetalleViewModel>> GetDocumentosAnexosAsync(int alumnoId);

        /// <summary>
        /// Verifica si todos los documentos obligatorios de matrícula están cargados.
        /// </summary>
        Task<VerificacionDocumentosResult> VerificarDocumentosObligatoriosAsync(int alumnoId, int anioEscolarId);

        /// <summary>
        /// Sube un documento de matrícula (EsAnexo=false) para un alumno y año escolar.
        /// </summary>
        Task<int> SubirDocumentoMatriculaAsync(int alumnoId, int tipoDocumentoId, int anioEscolarId,
            HttpPostedFileBase archivo, string observacion, string createdBy);

        /// <summary>
        /// Sube un documento anexo (EsAnexo=true) para un alumno.
        /// </summary>
        Task<int> SubirDocumentoAnexoAsync(int alumnoId, int tipoDocumentoId,
            HttpPostedFileBase archivo, string observacion, string createdBy);

        /// <summary>
        /// Elimina (desactiva) un documento del alumno.
        /// </summary>
        Task<bool> EliminarDocumentoAsync(int documentoId, string deletedBy);

        /// <summary>
        /// Descarga el contenido físico de un documento.
        /// </summary>
        Task<(byte[] Contenido, string NombreArchivo, string MimeType)> DescargarDocumentoAsync(int documentoId);

        /// <summary>
        /// Obtiene los tipos de documentos de tipo Anexo disponibles.
        /// </summary>
        Task<List<SelectItemViewModel>> GetTiposDocumentoAnexoAsync();

        /// <summary>
        /// Obtiene la fotografía del alumno desde BD.
        /// </summary>
        Task<(byte[] Contenido, string MimeType)> GetFotoAlumnoAsync(int alumnoId);
    }
}
