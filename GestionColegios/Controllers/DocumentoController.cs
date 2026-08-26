using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class DocumentoController : DefaultController
    {
        private readonly IDocumentoService _documentoService;
        private readonly IMatriculaService _matriculaService;

        public DocumentoController(IDocumentoService documentoService, IMatriculaService matriculaService)
        {
            _documentoService = documentoService;
            _matriculaService = matriculaService;
        }

        // GET: /Documento/GetEstadoMatricula?alumnoId=5&anioEscolarId=3
        [HttpGet]
        public async Task<JsonResult> GetEstadoMatricula(int alumnoId, int anioEscolarId)
        {
            var lista = await _documentoService.GetEstadoDocumentosMatriculaAsync(alumnoId, anioEscolarId);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: /Documento/GetAnexos?alumnoId=5
        [HttpGet]
        public async Task<JsonResult> GetAnexos(int alumnoId)
        {
            var lista = await _documentoService.GetDocumentosAnexosAsync(alumnoId);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: /Documento/GetTiposAnexo
        [HttpGet]
        public async Task<JsonResult> GetTiposAnexo()
        {
            var lista = await _documentoService.GetTiposDocumentoAnexoAsync();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: /Documento/VerificarObligatorios?alumnoId=5&anioEscolarId=3
        [HttpGet]
        public async Task<JsonResult> VerificarObligatorios(int alumnoId, int anioEscolarId)
        {
            var resultado = await _documentoService.VerificarDocumentosObligatoriosAsync(alumnoId, anioEscolarId);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // POST: /Documento/SubirMatricula
        [HttpPost]
        public async Task<JsonResult> SubirMatricula(SubirDocumentoViewModel model)
        {
            if (Request.Files["Archivo"] == null || Request.Files["Archivo"].ContentLength == 0)
                return Json(new { success = false, message = "Debe seleccionar un archivo." });

            if (!model.AnioEscolarID.HasValue)
                return Json(new { success = false, message = "Debe indicar el año escolar." });

            model.Archivo = Request.Files["Archivo"];

            var id = await _documentoService.SubirDocumentoMatriculaAsync(
                model.AlumnoID,
                model.TipoDocumentoID,
                model.AnioEscolarID.Value,
                model.Archivo,
                model.Observacion,
                UserEmail());

            if (id > 0)
            {
                // Verificar si con este nuevo documento se completan los obligatorios
                // y promover la matrícula de Pre-matriculado a Vigente si corresponde
                await _matriculaService.ActualizarEstadoSegunDocumentosAsync(
                    model.AlumnoID, model.AnioEscolarID.Value, UserEmail());

                return Json(new { success = true, message = "Documento cargado correctamente." });
            }

            return Json(new { success = false, message = "No se pudo guardar el documento." });
        }

        // POST: /Documento/SubirAnexo
        [HttpPost]
        public async Task<JsonResult> SubirAnexo(SubirDocumentoViewModel model)
        {
            if (Request.Files["Archivo"] == null || Request.Files["Archivo"].ContentLength == 0)
                return Json(new { success = false, message = "Debe seleccionar un archivo." });

            model.Archivo = Request.Files["Archivo"];

            var id = await _documentoService.SubirDocumentoAnexoAsync(
                model.AlumnoID,
                model.TipoDocumentoID,
                model.Archivo,
                model.Observacion,
                UserEmail());

            if (id > 0)
                return Json(new { success = true, message = "Documento anexo cargado correctamente." });

            return Json(new { success = false, message = "No se pudo guardar el documento." });
        }

        // POST: /Documento/Eliminar
        [HttpPost]
        public async Task<JsonResult> Eliminar(int documentoId)
        {
            var ok = await _documentoService.EliminarDocumentoAsync(documentoId, UserEmail());
            return Json(new { success = ok });
        }

        // GET: /Documento/Descargar/5
        [HttpGet]
        public async Task<ActionResult> Descargar(int id)
        {
            var (contenido, nombre, mimeType) = await _documentoService.DescargarDocumentoAsync(id);
            if (contenido == null) return HttpNotFound();
            return File(contenido, mimeType ?? "application/octet-stream", nombre);
        }

        // GET: /Documento/FotoAlumno/5
        [HttpGet]
        public async Task<ActionResult> FotoAlumno(int id)
        {
            var (contenido, mimeType) = await _documentoService.GetFotoAlumnoAsync(id);
            if (contenido == null) return HttpNotFound();
            return File(contenido, mimeType ?? "image/jpeg");
        }
    }
}
