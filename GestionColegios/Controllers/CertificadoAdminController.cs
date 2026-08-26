using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    /// <summary>
    /// Mantenedor de Certificados (Administración): datos del establecimiento,
    /// plantillas Word y firmantes. Solo accesible para Administradores.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class CertificadoAdminController : DefaultController
    {
        private readonly ICertificadoService _certificadoService;

        public CertificadoAdminController(ICertificadoService certificadoService)
        {
            _certificadoService = certificadoService;
        }

        // GET: /CertificadoAdmin
        public async Task<ActionResult> Index()
        {
            var vm = await _certificadoService.GetEstablecimientoAsync();
            return View(vm);
        }

        // ?? ESTABLECIMIENTO ??????????????????????????????????????????????
        [HttpGet]
        public async Task<JsonResult> GetEstablecimiento()
        {
            var est = await _certificadoService.GetEstablecimientoAsync();
            return Json(est, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GuardarEstablecimiento(EstablecimientoViewModel model)
        {
            try
            {
                var ok = await _certificadoService.GuardarEstablecimientoAsync(model, GetCurrentUsername());
                return Json(new { success = ok, message = ok ? "Datos del establecimiento guardados." : "No se pudo guardar." });
            }
            catch (DbEntityValidationException ex)
            {
                var detalle = string.Join(" ", ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                return Json(new { success = false, message = "Datos inválidos. " + detalle });
            }
            catch (Exception ex)
            {
                var detalle = ex.GetBaseException().Message;
                return Json(new { success = false, message = "No se pudo guardar: " + detalle });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SubirLogo()
        {
            var archivo = Request.Files["Logo"];
            if (archivo == null || archivo.ContentLength == 0)
                return Json(new { success = false, message = "Debe seleccionar una imagen (PNG o JPG)." });

            var ok = await _certificadoService.SubirLogoAsync(archivo, GetCurrentUsername());
            return Json(new { success = ok, message = ok ? "Logo actualizado." : "Formato inválido. Use PNG o JPG." });
        }

        // GET: /CertificadoAdmin/Logo  (preview del logo actual)
        [HttpGet]
        public async Task<ActionResult> Logo()
        {
            var (contenido, mime) = await _certificadoService.GetLogoAsync();
            if (contenido == null) return HttpNotFound();
            return File(contenido, mime);
        }

        // ?? PLANTILLAS ???????????????????????????????????????????????????
        [HttpGet]
        public async Task<JsonResult> GetPlantillas()
        {
            var lista = await _certificadoService.GetPlantillasAsync();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SubirPlantilla(int plantillaId)
        {
            var archivo = Request.Files["Archivo"];
            if (archivo == null || archivo.ContentLength == 0)
                return Json(new { success = false, message = "Debe seleccionar un archivo .docx." });

            var ok = await _certificadoService.SubirPlantillaAsync(plantillaId, archivo, GetCurrentUsername());
            return Json(new { success = ok, message = ok ? "Plantilla cargada correctamente." : "Formato inválido. Use un archivo Word (.docx)." });
        }

        // GET: /CertificadoAdmin/DescargarPlantilla?plantillaId=1  (descarga el .docx actual)
        [HttpGet]
        public async Task<ActionResult> DescargarPlantilla(int plantillaId)
        {
            var (contenido, nombreArchivo) = await _certificadoService.DescargarPlantillaAsync(plantillaId);
            if (contenido == null)
                return new HttpStatusCodeResult(404, "La plantilla aun no tiene un archivo cargado.");

            const string mime =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            return File(contenido, mime, nombreArchivo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SetFirmanteDefecto(int plantillaId, int? firmanteId)
        {
            var ok = await _certificadoService.SetFirmanteDefectoAsync(plantillaId, firmanteId, GetCurrentUsername());
            return Json(new { success = ok });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SetPlantillaActiva(int plantillaId, bool activa)
        {
            var ok = await _certificadoService.SetPlantillaActivaAsync(plantillaId, activa, GetCurrentUsername());
            return Json(new { success = ok });
        }

        // ?? FIRMANTES ????????????????????????????????????????????????????
        [HttpGet]
        public async Task<JsonResult> GetFirmantes()
        {
            var lista = await _certificadoService.GetFirmantesAsync();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GuardarFirmante(FirmanteGuardarViewModel model)
        {
            var id = await _certificadoService.GuardarFirmanteAsync(model, GetCurrentUsername());
            if (id == -1)
                return Json(new { success = false, message = "Nombre y cargo son obligatorios." });
            return Json(new { success = true, message = "Firmante guardado.", id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EliminarFirmante(int firmanteId)
        {
            var ok = await _certificadoService.EliminarFirmanteAsync(firmanteId, GetCurrentUsername());
            return Json(new { success = ok });
        }
    }
}
