using Gestionador.Interfaces;
using Gestionador.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProductosController : DefaultController
    {
        private readonly IProductosServices _productosService;
        public ProductosController(IProductosServices productosService)
        {
            _productosService = productosService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetConceptos()
        {
            dynamic servicios = await _productosService.GetProductosAsync();
            return Json(servicios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> _UpsertConcepto(int? id)
        {
            ConceptoRequest model;
            if (id.HasValue)
            {
                var concepto = await _productosService.GetProductoById(id.Value);
                model = new ConceptoRequest
                {
                    ID = concepto.ID,
                    CategoriaID = concepto.CategoriaID,
                    Concepto = concepto.Concepto,
                    IsActive = concepto.IsActive,
                    IsNicho = concepto.IsNicho
                };
            }
            else
            {
                model = new ConceptoRequest();
                model.IsNicho = false;
                model.IsActive = true;                   
            }
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpsertProductoAsync(ConceptoRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _productosService.UpsertProductoAsync(request);

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