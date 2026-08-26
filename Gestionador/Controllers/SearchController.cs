using Gestionador.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize]
    [Route("search")]
    public class SearchController : DefaultController
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [Route("regiones")]
        public List<dynamic> GetRegiones()
        {
            dynamic regions = _searchService.GetRegiones();
            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetComunasByRegionId(int id)
        {
            var comunas = await _searchService.GetComunasByRegionIdAsync(id);
            return Json(comunas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCementerios()
        {
            dynamic cementerios = _searchService.GetCementerios();
            return Json(cementerios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSeccionConceptoById(int id)
        {
            dynamic productos = _searchService.GetSeccionConceptoById(id);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConceptosBySectorId(int id)
        {
            dynamic productos = _searchService.GetConceptosBySectorId(id);
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFormasPago()
        {
            dynamic formasPago = _searchService.GetFormasPago();
            return Json(formasPago, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFormaPagoById(int id)
        {
            return Json(_searchService.GetFormaPagoById(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCausas()
        {
            dynamic causas = _searchService.GetCausas();
            return Json(causas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCausasAdmin()
        {
            dynamic causas = _searchService.GetCausasAdmin();
            return Json(causas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaestros()
        {
            dynamic maestros = _searchService.GetMaestros();
            return Json(maestros, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaestrosAdmin()
        {
            dynamic maestros = _searchService.GetMaestrosAdmin();
            return Json(maestros, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetValorMonedasAdmin()
        {
            dynamic causas = _searchService.GetValorMonedasAdmin();
            return Json(causas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTipoMonedas()
        {
            dynamic causas = _searchService.GetTipoMonedas();
            return Json(causas, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetParrocosAdmin()
        {
            dynamic parrocos = _searchService.GetParrocos();
            return Json(parrocos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBancos()
        {
            dynamic bancos = _searchService.GetBancos();
            return Json(bancos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLugarDefuncion()
        {
            dynamic lugar = _searchService.GetLugarDefuncion();
            return Json(lugar, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLugarDefuncionAdmin()
        {
            dynamic lugar = _searchService.GetLugarDefuncionAdmin();
            return Json(lugar, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCompradorByRut(string rut)
        {
            dynamic persona = _searchService.GetPersonaByRut(rut);
            if (persona == null)
            {
                return Json(new { Success = false, Message = "No se encontró una persona con el RUT proporcionado." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, Data = persona }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPatios()
        {
            dynamic patios = _searchService.GetPatios();
            return Json(patios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetSectoresByIdAsync(int id)
        {
            var sectores = await _searchService.GetSectoresByIdAsync(id);
            return Json(sectores, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetServiciosByConceptoId(int id, int seccionId)
        {
            var servicios = _searchService.GetServiciosByConceptoId(id, seccionId);
            return Json(servicios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPatiosByCementerioID(int id)
        {
            var patios = _searchService.GetPatiosByCementerioID(id);
            return Json(patios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetServiciosCategories(int id, int seccionId)
        {
            return Json(_searchService.GetServiciosCategories(id, seccionId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMantencionesCategories(int id, int seccionId)
        {
            return Json(_searchService.GetMantencionesCategories(id, seccionId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetConceptosServiciosBySeccionConceptoId(int seccionID, int conceptoId)
        {
            return Json(_searchService.GetConceptosServiciosBySeccionConceptoId(seccionID, conceptoId));
        }

        [HttpGet]
        public JsonResult GetAnios()
        {
            dynamic anios = _searchService.GetAnios();
            return Json(anios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCategorias()
        {
            dynamic categorias = _searchService.GetCategorias(new int[] { 2, 3, 4, 5, 6 });
            return Json(categorias, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTipoAdministrador()
        {
            dynamic categorias = _searchService.GetTipoAdministrador();
            return Json(categorias, JsonRequestBehavior.AllowGet);
        }
    }
}