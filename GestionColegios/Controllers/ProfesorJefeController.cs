using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class ProfesorJefeController : DefaultController
    {
        private readonly IProfesorJefeService _service;

        public ProfesorJefeController(IProfesorJefeService service)
        {
            _service = service;
        }

        // GET: /ProfesorJefe
        public async Task<ActionResult> Index()
        {
            ViewBag.EstadosProfesor = await _service.GetEstadosProfesorAsync();
            return View();
        }

        // POST: /ProfesorJefe/GetProfesores
        [HttpPost]
        public async Task<JsonResult> GetProfesores(DataSourceRequest request)
        {
            var result = await _service.GetProfesoresAsync(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProfesorJefe/GetEstadosProfesor
        [HttpGet]
        public async Task<JsonResult> GetEstadosProfesor()
        {
            var estados = await _service.GetEstadosProfesorAsync();
            var result  = estados.Select(e => new { EstadoProfesor = e.Texto, ID = e.ID });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProfesorJefe/Crear
        public async Task<ActionResult> Crear()
        {
            var vm = new ProfesorJefeViewModel
            {
                EstadoProfesorID   = 1,
                EstadosDisponibles = await _service.GetEstadosProfesorAsync()
            };
            return View("Form", vm);
        }

        // POST: /ProfesorJefe/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(ProfesorJefeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Revise los campos obligatorios." });
                model.EstadosDisponibles = await _service.GetEstadosProfesorAsync();
                return View("Form", model);
            }

            var result = await _service.CreateAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "El RUT ingresado ya se encuentra registrado." });
                ModelState.AddModelError("Rut", "El RUT ya se encuentra registrado.");
                model.EstadosDisponibles = await _service.GetEstadosProfesorAsync();
                return View("Form", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Profesor jefe registrado correctamente.", redirectUrl = Url.Action("Index") });
            return RedirectToAction("Index");
        }

        // GET: /ProfesorJefe/Editar/5
        public async Task<ActionResult> Editar(int id)
        {
            var vm = await _service.GetByIdAsync(id);
            if (vm == null) return HttpNotFound();
            return View("Form", vm);
        }

        // POST: /ProfesorJefe/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ProfesorJefeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Revise los campos obligatorios." });
                model.EstadosDisponibles = await _service.GetEstadosProfesorAsync();
                return View("Form", model);
            }

            var result = await _service.UpdateAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "El RUT ingresado ya está registrado por otro profesor." });
                ModelState.AddModelError("Rut", "El RUT ya está registrado por otro profesor.");
                model.EstadosDisponibles = await _service.GetEstadosProfesorAsync();
                return View("Form", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Profesor jefe actualizado correctamente.", redirectUrl = Url.Action("Index") });
            return RedirectToAction("Index");
        }

        // GET: /ProfesorJefe/ValidarRut
        [HttpGet]
        public async Task<JsonResult> ValidarRut(string rut, int profesorId = 0)
        {
            var duplicado = await _service.RutExisteAsync(rut, profesorId);
            return Json(new { duplicado }, JsonRequestBehavior.AllowGet);
        }

        // GET: /ProfesorJefe/GetVigentes  (para el select del año escolar)
        [HttpGet]
        public async Task<JsonResult> GetVigentes()
        {
            var request = new DataSourceRequest { Take = 500, Skip = 0 };
            var result  = await _service.GetProfesoresAsync(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
