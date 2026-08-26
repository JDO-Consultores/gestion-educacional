using GestionColegios.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    /// <summary>
    /// Generación on-demand de certificados en Word para un alumno.
    /// Disponible para usuarios autenticados (igual que los documentos).
    /// </summary>
    [Authorize]
    public class CertificadoController : DefaultController
    {
        private readonly ICertificadoService _certificadoService;

        public CertificadoController(ICertificadoService certificadoService)
        {
            _certificadoService = certificadoService;
        }

        // GET: /Certificado/GetPlantillas
        [HttpGet]
        public async Task<JsonResult> GetPlantillas()
        {
            var lista = await _certificadoService.GetPlantillasDisponiblesAsync();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: /Certificado/GetFirmantes
        [HttpGet]
        public async Task<JsonResult> GetFirmantes()
        {
            var lista = await _certificadoService.GetFirmantesAsync(soloActivos: true);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // GET: /Certificado/Generar?plantillaId=1&alumnoId=5&firmanteId=2
        [HttpGet]
        public async Task<ActionResult> Generar(int plantillaId, int alumnoId, int? firmanteId)
        {
            var (contenido, nombreArchivo) =
                await _certificadoService.GenerarCertificadoAsync(plantillaId, alumnoId, firmanteId);

            if (contenido == null)
                return new HttpStatusCodeResult(404,
                    "No se pudo generar el certificado. Verifique que la plantilla esté cargada.");

            const string mime =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            return File(contenido, mime, nombreArchivo);
        }
    }
}
