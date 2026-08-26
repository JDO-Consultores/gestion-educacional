using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class AnioEscolarController : DefaultController
    {
        private readonly IAnioEscolarService _service;

        public AnioEscolarController(IAnioEscolarService service)
        {
            _service = service;
        }

        // GET: /AnioEscolar
        public ActionResult Index()
        {
            return View();
        }

        // POST: /AnioEscolar/GetAniosEscolares
        [HttpPost]
        public async Task<JsonResult> GetAniosEscolares(DataSourceRequest request)
        {
            var result = await _service.GetAniosEscolaresAsync(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /AnioEscolar/Detalle/5
        public async Task<ActionResult> Detalle(int id)
        {
            var vm = await _service.GetDetalleAsync(id);
            if (vm == null) return HttpNotFound();
            return View(vm);
        }

        // GET: /AnioEscolar/Crear
        public async Task<ActionResult> Crear()
        {
            var vm = await _service.GetFormAnioAsync();
            return View("FormAnioEscolar", vm);
        }

        // POST: /AnioEscolar/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(AnioEscolarFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos." });
                model.Establecimientos = (await _service.GetFormAnioAsync()).Establecimientos;
                return View("FormAnioEscolar", model);
            }

            var result = await _service.CreateAnioEscolarAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Ya existe un año escolar registrado para ese año y establecimiento." });
                ModelState.AddModelError("Anio", "Ya existe un año escolar para ese período y establecimiento.");
                model.Establecimientos = (await _service.GetFormAnioAsync()).Establecimientos;
                return View("FormAnioEscolar", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Año escolar creado correctamente.", redirectUrl = Url.Action("Detalle", new { id = result }) });
            return RedirectToAction("Detalle", new { id = result });
        }

        // GET: /AnioEscolar/Editar/5
        public async Task<ActionResult> Editar(int id)
        {
            var vm = await _service.GetFormAnioAsync(id);
            if (vm == null) return HttpNotFound();
            return View("FormAnioEscolar", vm);
        }

        // POST: /AnioEscolar/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(AnioEscolarFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos." });
                model.Establecimientos = (await _service.GetFormAnioAsync()).Establecimientos;
                return View("FormAnioEscolar", model);
            }

            var result = await _service.UpdateAnioEscolarAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Ya existe un año escolar registrado para ese año y establecimiento." });
                ModelState.AddModelError("Anio", "Ya existe un año escolar para ese período y establecimiento.");
                model.Establecimientos = (await _service.GetFormAnioAsync()).Establecimientos;
                return View("FormAnioEscolar", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Año escolar actualizado correctamente.", redirectUrl = Url.Action("Detalle", new { id = result }) });
            return RedirectToAction("Detalle", new { id = result });
        }

        // POST: /AnioEscolar/CerrarReabrir
        [HttpPost]
        public async Task<JsonResult> CerrarReabrir(int id)
        {
            var result = await _service.CerrarReobrirAnioAsync(id, GetCurrentUsername());
            return Json(new { success = result > 0 });
        }

        // POST: /AnioEscolar/MarcarActivo
        [HttpPost]
        public async Task<JsonResult> MarcarActivo(int id)
        {
            var result = await _service.MarcarComoActivoAsync(id, GetCurrentUsername());
            if (result == -1)
                return Json(new { success = false, message = "No se puede activar un año escolar cerrado." });
            return Json(new { success = result > 0 });
        }

        // ?? Cierre de año: promoción de alumnos ???????????????????????????

        // GET: /AnioEscolar/Promocion/5
        public async Task<ActionResult> Promocion(int id)
        {
            var vm = await _service.GetDetalleAsync(id);
            if (vm == null) return HttpNotFound();
            ViewBag.IsAdmin = IsAdmin();
            return View(vm);
        }

        // POST: /AnioEscolar/GetPromocionAlumnos?anioEscolarId=5
        [HttpPost]
        public async Task<JsonResult> GetPromocionAlumnos(int anioEscolarId, DataSourceRequest request)
        {
            var result = await _service.GetPromocionAlumnosAsync(anioEscolarId, request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /AnioEscolar/RegistrarPromocion
        [HttpPost]
        public async Task<JsonResult> RegistrarPromocion(RegistrarPromocionViewModel model)
        {
            var result = await _service.RegistrarPromocionAsync(model, GetCurrentUsername());
            if (result == 0)
                return Json(new { success = false, message = "No se encontró la matrícula." });
            if (result == -1)
                return Json(new { success = false, message = "Debe indicar un resultado válido (Promovido o No Promovido)." });
            if (result == -2)
                return Json(new { success = false, message = "Para 'No Promovido' debe indicar Motivo, Decreto y Glosa." });
            return Json(new { success = true, message = "Resultado de promoción registrado correctamente." });
        }

        // GET: /AnioEscolar/GetCursosConPendientes?anioEscolarId=5
        [HttpGet]
        public async Task<JsonResult> GetCursosConPendientes(int anioEscolarId)
        {
            var cursos = await _service.GetCursosConPendientesAsync(anioEscolarId);
            return Json(cursos, JsonRequestBehavior.AllowGet);
        }

        // POST: /AnioEscolar/PromoverCurso
        [HttpPost]
        public async Task<JsonResult> PromoverCurso(int anioEscolarId, int cursoId)
        {
            var total = await _service.PromoverCursoAsync(anioEscolarId, cursoId, GetCurrentUsername());
            if (total == 0)
                return Json(new { success = false, message = "El curso no tiene alumnos pendientes de promoción." });
            return Json(new { success = true, message = $"{total} alumno(s) marcados como Promovido." });
        }

        // POST: /AnioEscolar/AutorizarMatricula
        [HttpPost]
        public async Task<JsonResult> AutorizarMatricula(AutorizarMatriculaViewModel model)
        {
            var (ok, error) = await _service.AutorizarMatriculaCanceladaAsync(model, GetCurrentUsername());
            if (!ok)
                return Json(new { success = false, message = error });
            return Json(new { success = true, message = "Autorización concedida. El alumno podrá matricularse el próximo año." });
        }

        // ?? Cursos ????????????????????????????????????????????????????????

        // GET: /AnioEscolar/CrearCurso?anioEscolarId=5
        public async Task<ActionResult> CrearCurso(int anioEscolarId)
        {
            var vm = await _service.GetFormCursoAsync(anioEscolarId);
            if (vm == null) return HttpNotFound();
            return View("FormCurso", vm);
        }

        // POST: /AnioEscolar/CrearCurso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearCurso(CursoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                var form = await _service.GetFormCursoAsync(model.AnioEscolarID);
                model.Grados = form.Grados;
                model.Profesores = form.Profesores;
                return View("FormCurso", model);
            }

            var result = await _service.CreateCursoAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Ya existe un curso con ese grado y letra para este año escolar." });
                ModelState.AddModelError("GradoID", "Ya existe ese curso en el año escolar.");
                var form = await _service.GetFormCursoAsync(model.AnioEscolarID);
                model.Grados = form.Grados;
                model.Profesores = form.Profesores;
                return View("FormCurso", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Curso creado correctamente.", redirectUrl = Url.Action("Detalle", new { id = model.AnioEscolarID }) });
            return RedirectToAction("Detalle", new { id = model.AnioEscolarID });
        }

        // GET: /AnioEscolar/EditarCurso/5
        public async Task<ActionResult> EditarCurso(int id)
        {
            var curso = await _service.GetFormCursoAsync(0, id);
            if (curso == null) return HttpNotFound();

            // Recargar con el año correcto
            var vm = await _service.GetFormCursoAsync(curso.AnioEscolarID, id);
            return View("FormCurso", vm);
        }

        // POST: /AnioEscolar/EditarCurso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarCurso(CursoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                var form = await _service.GetFormCursoAsync(model.AnioEscolarID);
                model.Grados = form.Grados;
                model.Profesores = form.Profesores;
                return View("FormCurso", model);
            }

            var result = await _service.UpdateCursoAsync(model, GetCurrentUsername());
            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Ya existe un curso con ese grado y letra para este año escolar." });
                ModelState.AddModelError("GradoID", "Ya existe ese curso en el año escolar.");
                var form = await _service.GetFormCursoAsync(model.AnioEscolarID);
                model.Grados = form.Grados;
                model.Profesores = form.Profesores;
                return View("FormCurso", model);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Curso actualizado correctamente.", redirectUrl = Url.Action("Detalle", new { id = model.AnioEscolarID }) });
            return RedirectToAction("Detalle", new { id = model.AnioEscolarID });
        }

        // POST: /AnioEscolar/EliminarCurso
        [HttpPost]
        public async Task<JsonResult> EliminarCurso(int cursoId)
        {
            var result = await _service.EliminarCursoAsync(cursoId, GetCurrentUsername());
            if (result == -1)
                return Json(new { success = false, message = "No se puede eliminar un curso que tiene alumnos matriculados." });
            return Json(new { success = result > 0, message = result > 0 ? "Curso eliminado." : "No se encontró el curso." });
        }

        // GET: /AnioEscolar/GetProfesores
        [HttpGet]
        public async Task<JsonResult> GetProfesores()
        {
            var items = await _service.GetProfesoresAsync();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}
