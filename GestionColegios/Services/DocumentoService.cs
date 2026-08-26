using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GestionColegios.Services
{
    public class DocumentoService : BaseServices, IDocumentoService
    {

        public DocumentoService(
            Entities dbContext,
            IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        // Orden de categorías por importancia
        private static readonly Dictionary<string, int> _ordenCategoria = new Dictionary<string, int>
        {
            { "Reglamentario", 1 },
            { "Autorizacion",  2 },
            { "Interno",       3 }
        };

        public async Task<List<DocumentoMatriculaEstadoViewModel>> GetEstadoDocumentosMatriculaAsync(
            int alumnoId, int anioEscolarId)
        {
            var tipos = await _dbContext.tbl_TipoDocumento
                .Where(t => t.IsActive && !t.EsAnexo)
                .ToListAsync();

            var cargados = await _dbContext.tbl_DocumentoAlumno
                .Where(d => d.AlumnoID == alumnoId
                         && d.AnioEscolarID == anioEscolarId
                         && d.IsActive)
                .ToListAsync();

            return tipos.Select(t =>
            {
                var doc       = cargados.FirstOrDefault(c => c.TipoDocumentoID == t.ID);
                var esCargado = doc != null;
                var catOrden  = _ordenCategoria.ContainsKey(t.Categoria ?? string.Empty)
                                    ? _ordenCategoria[t.Categoria]
                                    : 99;

                // 1=Obligatorio pendiente (más urgente)
                // 2=Obligatorio cargado
                // 3=Opcional pendiente
                // 4=Opcional cargado
                int ordenImp;
                if      ( t.Obligatorio && !esCargado) ordenImp = 1;
                else if ( t.Obligatorio &&  esCargado) ordenImp = 2;
                else if (!t.Obligatorio && !esCargado) ordenImp = 3;
                else                                   ordenImp = 4;

                return new DocumentoMatriculaEstadoViewModel
                {
                    TipoDocumentoID   = t.ID,
                    Nombre            = t.Nombre,
                    Categoria         = t.Categoria,
                    Obligatorio       = t.Obligatorio,
                    Cargado           = esCargado,
                    DocumentoAlumnoID = doc?.ID,
                    NombreArchivo     = doc?.NombreArchivo,
                    FechaCarga        = doc?.FechaCarga,
                    OrdenImportancia  = ordenImp,
                    OrdenCategoria    = catOrden
                };
            })
            .OrderBy(d => d.OrdenImportancia)
            .ThenBy(d => d.OrdenCategoria)
            .ThenBy(d => d.Nombre)
            .ToList();
        }

        public async Task<List<DocumentoAlumnoDetalleViewModel>> GetDocumentosAnexosAsync(int alumnoId)
        {
            return await _dbContext.tbl_DocumentoAlumno
                .Where(d => d.AlumnoID == alumnoId
                         && d.IsActive
                         && d.tbl_TipoDocumento.EsAnexo)
                .OrderByDescending(d => d.FechaCarga)
                .Select(d => new DocumentoAlumnoDetalleViewModel
                {
                    ID              = d.ID,
                    AlumnoID        = d.AlumnoID,
                    TipoDocumentoID = d.TipoDocumentoID,
                    TipoDocumento   = d.tbl_TipoDocumento.Nombre,
                    Categoria       = d.tbl_TipoDocumento.Categoria,
                    Obligatorio     = d.tbl_TipoDocumento.Obligatorio,
                    EsAnexo         = d.tbl_TipoDocumento.EsAnexo,
                    NombreArchivo   = d.NombreArchivo,
                    FechaCarga      = d.FechaCarga,
                    Observacion     = d.Observacion,
                    AnioEscolarID   = null
                })
                .ToListAsync();
        }

        public async Task<VerificacionDocumentosResult> VerificarDocumentosObligatoriosAsync(
            int alumnoId, int anioEscolarId)
        {
            var tiposObligatorios = await _dbContext.tbl_TipoDocumento
                .Where(t => t.IsActive && !t.EsAnexo && t.Obligatorio)
                .Select(t => new { t.ID, t.Nombre })
                .ToListAsync();

            var cargados = await _dbContext.tbl_DocumentoAlumno
                .Where(d => d.AlumnoID == alumnoId
                         && d.AnioEscolarID == anioEscolarId
                         && d.IsActive)
                .Select(d => d.TipoDocumentoID)
                .ToListAsync();

            var faltantes = tiposObligatorios
                .Where(t => !cargados.Contains(t.ID))
                .Select(t => t.Nombre)
                .ToList();

            return new VerificacionDocumentosResult
            {
                TodosObligatoriosCargados = faltantes.Count == 0,
                FaltanDocumentos          = faltantes
            };
        }

        public async Task<int> SubirDocumentoMatriculaAsync(int alumnoId, int tipoDocumentoId,
            int anioEscolarId, HttpPostedFileBase archivo, string observacion, string createdBy)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return 0;

            var tipo = await _dbContext.tbl_TipoDocumento.FindAsync(tipoDocumentoId);
            if (tipo == null || tipo.EsAnexo) return 0;

            // Si ya existe uno activo para este tipo/año, desactivarlo (reemplazar)
            var existente = await _dbContext.tbl_DocumentoAlumno
                .FirstOrDefaultAsync(d => d.AlumnoID == alumnoId
                                       && d.TipoDocumentoID == tipoDocumentoId
                                       && d.AnioEscolarID == anioEscolarId
                                       && d.IsActive);
            if (existente != null)
                existente.IsActive = false;

            // Leer contenido en memoria
            byte[] contenido;
            using (var ms = new MemoryStream())
            {
                archivo.InputStream.CopyTo(ms);
                contenido = ms.ToArray();
            }
            var mimeType = ObtenerMimeType(archivo.FileName);

            var doc = new tbl_DocumentoAlumno
            {
                AlumnoID        = alumnoId,
                TipoDocumentoID = tipoDocumentoId,
                AnioEscolarID   = anioEscolarId,
                NombreArchivo   = Path.GetFileName(archivo.FileName),
                Contenido       = contenido,
                MimeType        = mimeType,
                FechaCarga      = DateTime.Now,
                Observacion     = observacion,
                IsActive        = true,
                CreatedDate     = DateTime.UtcNow,
                CreatedBy       = createdBy
            };

            _dbContext.tbl_DocumentoAlumno.Add(doc);

            // Si es Fotografía, actualizar el avatar del alumno
            if (EsFotografia(tipo.Nombre))
                await SincronizarFotoAlumnoAsync(alumnoId, contenido, mimeType);

            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Documento", alumnoId, "CARGÓ DOCUMENTO DE MATRÍCULA",
                createdBy, $"Tipo: {tipo.Nombre} | Archivo: {doc.NombreArchivo}");

            return doc.ID;
        }

        public async Task<int> SubirDocumentoAnexoAsync(int alumnoId, int tipoDocumentoId,
            HttpPostedFileBase archivo, string observacion, string createdBy)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return 0;

            var tipo = await _dbContext.tbl_TipoDocumento.FindAsync(tipoDocumentoId);
            if (tipo == null || !tipo.EsAnexo) return 0;

            // Leer contenido en memoria
            byte[] contenido;
            using (var ms = new MemoryStream())
            {
                archivo.InputStream.CopyTo(ms);
                contenido = ms.ToArray();
            }
            var mimeType = ObtenerMimeType(archivo.FileName);

            var doc = new tbl_DocumentoAlumno
            {
                AlumnoID        = alumnoId,
                TipoDocumentoID = tipoDocumentoId,
                AnioEscolarID   = null,
                NombreArchivo   = Path.GetFileName(archivo.FileName),
                Contenido       = contenido,
                MimeType        = mimeType,
                FechaCarga      = DateTime.Now,
                Observacion     = observacion,
                IsActive        = true,
                CreatedDate     = DateTime.UtcNow,
                CreatedBy       = createdBy
            };

            _dbContext.tbl_DocumentoAlumno.Add(doc);

            // Si es Fotografía, actualizar el avatar del alumno
            if (EsFotografia(tipo.Nombre))
                await SincronizarFotoAlumnoAsync(alumnoId, contenido, mimeType);

            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Documento", alumnoId, "CARGÓ DOCUMENTO ANEXO",
                createdBy, $"Tipo: {tipo.Nombre} | Archivo: {doc.NombreArchivo}");

            return doc.ID;
        }

        public async Task<bool> EliminarDocumentoAsync(int documentoId, string deletedBy)
        {
            var doc = await _dbContext.tbl_DocumentoAlumno
                .Include(d => d.tbl_TipoDocumento)
                .FirstOrDefaultAsync(d => d.ID == documentoId);
            if (doc == null) return false;

            doc.IsActive = false;

            // Si era la Fotografía, limpiar avatar del alumno
            if (EsFotografia(doc.tbl_TipoDocumento?.Nombre))
                await SincronizarFotoAlumnoAsync(doc.AlumnoID, null, null);

            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Documento", doc.AlumnoID, "ELIMINÓ DOCUMENTO",
                deletedBy, $"Tipo: {doc.tbl_TipoDocumento?.Nombre} | Archivo: {doc.NombreArchivo}");

            return true;
        }

        public async Task<(byte[] Contenido, string NombreArchivo, string MimeType)> DescargarDocumentoAsync(
            int documentoId)
        {
            var doc = await _dbContext.tbl_DocumentoAlumno
                .FirstOrDefaultAsync(d => d.ID == documentoId && d.IsActive);
            if (doc == null || doc.Contenido == null) return (null, null, null);

            var mimeType = !string.IsNullOrEmpty(doc.MimeType)
                ? doc.MimeType
                : ObtenerMimeType(doc.NombreArchivo);

            return (doc.Contenido, doc.NombreArchivo, mimeType);
        }

        public async Task<List<SelectItemViewModel>> GetTiposDocumentoAnexoAsync()
        {
            return await _dbContext.tbl_TipoDocumento
                .Where(t => t.IsActive && t.EsAnexo)
                .OrderBy(t => t.Nombre)
                .Select(t => new SelectItemViewModel { ID = t.ID, Texto = t.Nombre })
                .ToListAsync();
        }

        public async Task<(byte[] Contenido, string MimeType)> GetFotoAlumnoAsync(int alumnoId)
        {
            var alumno = await _dbContext.tbl_Alumno.FindAsync(alumnoId);
            if (alumno?.FotoContenido == null) return (null, null);
            return (alumno.FotoContenido, alumno.FotoMimeType ?? "image/jpeg");
        }

        private static bool EsFotografia(string nombreTipoDoc)
            => !string.IsNullOrEmpty(nombreTipoDoc) &&
               nombreTipoDoc.IndexOf("Fotograf", StringComparison.OrdinalIgnoreCase) >= 0;

        private async Task SincronizarFotoAlumnoAsync(int alumnoId, byte[] contenido, string mimeType)
        {
            var alumno = await _dbContext.tbl_Alumno.FindAsync(alumnoId);
            if (alumno == null) return;
            alumno.FotoContenido = contenido;
            alumno.FotoMimeType  = mimeType;
        }

        private static string ObtenerMimeType(string nombreArchivo)
        {
            if (string.IsNullOrEmpty(nombreArchivo)) return "application/octet-stream";
            var ext = Path.GetExtension(nombreArchivo).ToLowerInvariant();
            switch (ext)
            {
                case ".pdf":  return "application/pdf";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png":  return "image/png";
                case ".gif":  return "image/gif";
                case ".doc":  return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":  return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:      return "application/octet-stream";
            }
        }
    }
}
