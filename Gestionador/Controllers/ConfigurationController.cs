using Gestionador.Interfaces;
using Gestionador.Models;
using Gestionador.Responses;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ConfigurationController : DefaultController
    {
        private readonly ISearchService _searchService;
        private readonly IConfigService _configService;
        public ConfigurationController(IConfigService configService, ISearchService searchService)
        {
            _searchService = searchService;
            _configService = configService;
        }

        [HttpGet]
        public ActionResult Causas()
        {
            return View("Causas/Causas");
        }

        [HttpGet]
        public ActionResult LugarDefuncion()
        {
            return View("LugarDefuncion/LugarDefuncion");
        }

        [HttpGet]
        public ActionResult TipoAdministrador()
        {
            return View("TipoAdministrador/TipoAdministrador");
        }

        [HttpGet]
        public ActionResult TipoMonedas()
        {
            return View("TipoMonedas/TipoMonedas");
        }

        [HttpGet]
        public ActionResult Maestros()
        {            
            return View("Maestros/Maestros");
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertCausa(int? id)
        {
            CausaRequest model;
            if (id.HasValue)
            {
                var causa = await _configService.GetCausaById(id.Value);
                model = new CausaRequest
                {
                    Causa = causa.Causa,
                    ID = id.Value,
                    IsActive = causa.IsActive
                };
            }
            else
            {
                model = new CausaRequest();
                model.IsActive = true;
            }
            return PartialView("Causas/_UpsertCausa", model);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertMaestro(int? id)
        {
            PersonaViewModel model;
            ViewBag.regiones = _searchService.GetRegiones();
            if (id.HasValue)
            {
                PersonaResponse maestro = await _configService.GetMaestroById(id.Value);
                return PartialView("Maestros/_UpsertMaestroEdit", maestro);
            }
            else
            {
                model = new PersonaViewModel();
                model.IsActive = true;
            }
            return PartialView("Maestros/_UpsertMaestro", model);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertLugarDefuncion(int? id)
        {
            LugarDefuncionRequest model;
            if (id.HasValue)
            {
                var lugar = await _configService.GetLugerDefuncionById(id.Value);
                model = new LugarDefuncionRequest
                {
                    Lugar = lugar.Lugar,
                    ID = id.Value,
                    IsActive = lugar.IsActive
                };
            }
            else
            {
                model = new LugarDefuncionRequest();
                model.IsActive = true;
            }
            return PartialView("LugarDefuncion/_UpsertLugarDefuncion", model);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertParrocos(int? id)
        {
            ParrocoRequest model;
            if (id.HasValue)
            {
                var parroco = await _configService.GetParrocoById(id.Value);
                model = new ParrocoRequest
                {
                    ID = parroco.ID,
                    TipoAdministradorID = parroco.TipoAdministradorID,
                    Nombre = parroco.NombreParroco,
                    Rut = parroco.RutParroco,
                    IsActive = parroco.IsActive
                };
            }
            else
            {
                model = new ParrocoRequest();
                model.IsActive = true;                
            }
            ViewBag.TipoAdministrador = _searchService.GetTipoAdministrador();
            return PartialView("TipoAdministrador/_UpsertParrocos", model);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertValorMoneda(int? id)
        {
            ValorMonedaRequest model;
            if (id.HasValue)
            {
                var valorMoneda = await _configService.GetValorMonedaById(id.Value);
                model = new ValorMonedaRequest
                {
                    ID = id.Value,
                    Fecha = valorMoneda.Fecha,
                    TipoMonedaID = valorMoneda.TipoMonedaResponse.ID,
                    Valor = valorMoneda.Valor
                };
            }
            else
            {
                model = new ValorMonedaRequest();
                model.Fecha = System.DateTime.Now;
            }
            ViewBag.TipoMonedas = _searchService.GetTipoMonedas();
            return PartialView("TipoMonedas/_UpsertValorMoneda", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertCausaAsync(CausaRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _configService.UpsertCausaAsync(request);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertMaestroAsync(PersonaViewModel request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _configService.UpsertMaestroAsync(request);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "El maestro ya se encuentra registrado." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertValorMonedaAsync(ValorMonedaRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _configService.UpsertTipoMonedaAsync(request);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "Ya existe un valor para la moneda ingresada." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertLugarDefuncionAsync(LugarDefuncionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _configService.UpsertLugarDefuncionAsync(request);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertParrocoAsync(ParrocoRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _configService.UpsertParrocoAsync(request);

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