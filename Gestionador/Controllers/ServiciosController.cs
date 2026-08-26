using Gestionador.Interfaces;
using Gestionador.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ServiciosController : DefaultController
    {
        private readonly ISearchService _searchService;
        private readonly IServiciosService _servicioService;

        public ServiciosController(IServiciosService serviciosInterface, ISearchService searchService)
        {
            _servicioService = serviciosInterface;
            _searchService = searchService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetServicios()
        {
            dynamic servicios = await _servicioService.GetServicios();
            return Json(servicios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertServicio(int? id)
        {
            ServicioRequest model;
            if (id.HasValue)
            {
                var servicio = await _servicioService.GetServicioById(id.Value);
                model = new ServicioRequest
                { 
                    ID = servicio.ID,
                    CategoriaID = servicio.CategoriaID,
                    Servicio = servicio.Servicio,
                    IsActive = servicio.IsActive
                };
            }
            else
            {
                model = new ServicioRequest();
            }
            ViewBag.Categorias = _searchService.GetCategorias(new int[] { 2, 3, 4, 5 });
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertServicioAsync(ServicioRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _servicioService.UpsertServicioAsync(request);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}