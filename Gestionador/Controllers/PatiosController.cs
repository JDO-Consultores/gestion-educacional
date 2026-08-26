using Gestionador.Interfaces;
using Gestionador.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PatiosController : DefaultController
    {
        private protected readonly IPatiosService _patiosServices;
        private protected readonly ISearchService _searchServices;

        public PatiosController(IPatiosService patiosServices, ISearchService searchServices)
        {
            _patiosServices = patiosServices;
            _searchServices = searchServices;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetPatios()
        {
            var patios = await _patiosServices.GetPatiosAsync();
            return Json(patios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetPatiosByCementerioId(int id)
        {
            var patios = await _patiosServices.GetPatiosByCementerioId(id);
            return Json(patios, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public async Task<JsonResult> GetCementeriosAsync()
        {
            var cementerios = await _patiosServices.GetCementeriosAsync();
            return Json(cementerios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult _AssignSeccionesByConcepto()
        {
            SeccionConceptoRequest model = new SeccionConceptoRequest();
            model.IsActive = true;
            ViewBag.Cementerios = _searchServices.GetCementerios();
            ViewBag.Conceptos = _searchServices.GetConceptosAsync();
            ViewBag.Servicios = _searchServices.GetServiciosById(new int[] { }, true);
            ViewBag.TipoMonedas = _searchServices.GetTipoMonedas();
            return PartialView(model);
        }

        [HttpGet]
        public async Task<ActionResult> _EditSeccionConcepto(int id)
        {
            var model = await _patiosServices.GetSeccionConceptoByIdAsync(id);

            ViewBag.Patios = _searchServices.GetPatios();
            ViewBag.Conceptos = _searchServices.GetConceptosAsync();
            ViewBag.Servicios = _searchServices.GetServiciosById(new int[] { }, true);
            ViewBag.TipoMonedas = _searchServices.GetTipoMonedas();
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult _CreatePatio()
        {
            ViewBag.Cementerios = _searchServices.GetCementerios();
            return PartialView();
        }

        [HttpGet]
        public async Task<ActionResult> _EditPatio(int id)
        {
            var model = await _patiosServices.GetPatioAsync(id);
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> CreatePatios(PatioRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _patiosServices.CreatePatiosAsync(request);
            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "Este patio ya existe." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditPatios(PatioRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _patiosServices.EditPatiosAsync(request);
            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateSeccionConcepto(SeccionConceptoRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _patiosServices.CreateSeccionConcepto(request);
            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "Este Producto ya se encuentra en el sector seleccionado." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditSeccionConcepto(SeccionConceptoRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _patiosServices.EditSeccionConceptoAsync(request);
            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente."}, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "Este Producto ya se encuentra en el sector seleccionado." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}