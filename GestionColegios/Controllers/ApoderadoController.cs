using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    [Authorize]
    public class ApoderadoController : DefaultController
    {
        private readonly IApoderadoService _service;

        public ApoderadoController(IApoderadoService service)
        {
            _service = service;
        }

        // GET: /Apoderado/Crear?alumnoId=5
        [HttpGet]
        public async Task<ActionResult> Crear(int alumnoId)
        {
            var vm = await _service.GetFormDataAsync(alumnoId);
            if (vm == null) return HttpNotFound();
            return View(vm);
        }

        // POST: /Apoderado/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(ApoderadoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refresh = await _service.GetFormDataAsync(model.AlumnoID);
                refresh.Apoderado          = model.Apoderado;
                refresh.ParentescoID       = model.ParentescoID;
                refresh.EsApoderadoTitular = model.EsApoderadoTitular;
                refresh.TipoApoderado      = model.TipoApoderado;
                return View(refresh);
            }

            var id = await _service.GuardarApoderadoAsync(model, GetCurrentUsername());
            if (id > 0)
            {
                TempData["Success"] = "Apoderado guardado correctamente.";
                return RedirectToAction("Ficha", "Alumnos", new { id = model.AlumnoID });
            }

            ModelState.AddModelError("", "No se pudo guardar el apoderado.");
            var vm = await _service.GetFormDataAsync(model.AlumnoID);
            return View(vm);
        }

        // GET: /Apoderado/Editar?alumnoApoderadoId=3&alumnoId=5
        [HttpGet]
        public async Task<ActionResult> Editar(int alumnoId, int alumnoApoderadoId)
        {
            var vm = await _service.GetFormDataAsync(alumnoId, alumnoApoderadoId);
            if (vm == null) return HttpNotFound();
            return View("Crear", vm);   // reutiliza la misma vista
        }

        // POST: /Apoderado/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ApoderadoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var refresh = await _service.GetFormDataAsync(model.AlumnoID, model.Apoderado.ID);
                return View("Crear", refresh);
            }

            await _service.GuardarApoderadoAsync(model, GetCurrentUsername());
            TempData["Success"] = "Apoderado actualizado correctamente.";
            return RedirectToAction("Ficha", "Alumnos", new { id = model.AlumnoID });
        }

        // POST: /Apoderado/Desvincular
        [HttpPost]
        public async Task<JsonResult> Desvincular(int alumnoApoderadoId)
        {
            var result = await _service.DesvincularApoderadoAsync(alumnoApoderadoId, GetCurrentUsername());
            return Json(new { success = result > 0 });
        }

        // GET: /Apoderado/BuscarPorRut?rut=12345678-9
        [HttpGet]
        public async Task<JsonResult> BuscarPorRut(string rut)
        {
            var apoderado = await _service.BuscarPorRutAsync(rut);
            return Json(apoderado, JsonRequestBehavior.AllowGet);
        }
    }
}
