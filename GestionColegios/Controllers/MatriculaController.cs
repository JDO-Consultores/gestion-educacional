using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class MatriculaController : DefaultController
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculaController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        // GET: /Matricula
        public async Task<ActionResult> Index(int? anioEscolarId)
        {
            var resumen = await _matriculaService.GetResumenAnioAsync(anioEscolarId);
            ViewBag.IsAdmin          = IsAdmin();
            ViewBag.EstadosMatricula = await _matriculaService.GetEstadosMatriculaAsync();
            return View(resumen);
        }

        // GET: /Matricula/GetEstadosMatricula
        [HttpGet]
        public async Task<JsonResult> GetEstadosMatricula()
        {
            var estados = await _matriculaService.GetEstadosMatriculaAsync();
            var result  = estados.Select(e => new { EstadoMatricula = e.Texto, ID = e.ID });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Matricula/GetMatriculas (listado general)
        [HttpPost]
        public async Task<JsonResult> GetMatriculasIndex(int? anioEscolarId, DataSourceRequest request)
        {
            var result = await _matriculaService.GetMatriculasAsync(anioEscolarId, request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Matricula/Historial?alumnoId=5
        public ActionResult Historial(int? alumnoId)
        {
            if (alumnoId == null) return HttpNotFound();
            ViewBag.AlumnoID = alumnoId.Value;
            ViewBag.IsAdmin = IsAdmin();
            return View();
        }

        // POST: /Matricula/GetMatriculas
        [HttpPost]
        public async Task<JsonResult> GetMatriculas(int alumnoId, DataSourceRequest request)
        {
            var result = await _matriculaService.GetMatriculasPorAlumnoAsync(alumnoId, request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Matricula/Crear?alumnoId=5
        public async Task<ActionResult> Crear(int? alumnoId)
        {
            if (alumnoId == null) return HttpNotFound();
            var formData = await _matriculaService.GetFormDataAsync(alumnoId.Value);
            if (formData == null) return HttpNotFound();
            return View(formData);
        }

        // POST: /Matricula/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(MatriculaFormViewModel form)
        {
            var model = form.Matricula;

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                var formData = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                formData.Matricula = model;
                return View(formData);
            }

            var result = await _matriculaService.CreateMatriculaAsync(model, UserEmail());

            if (result.MatriculaID == 0)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "El alumno no existe o no se encuentra activo." });
                ModelState.AddModelError("Matricula.AlumnoID",
                    "El alumno no existe o no se encuentra activo.");
                var formDataNotFound = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                if (formDataNotFound == null) return HttpNotFound();
                formDataNotFound.Matricula = model;
                return View(formDataNotFound);
            }

            if (result.MatriculaID == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "El alumno ya tiene una matrícula para ese año escolar." });
                ModelState.AddModelError("Matricula.AnioEscolarID",
                    "El alumno ya tiene una matrícula para ese año escolar.");
                var formData = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                formData.Matricula = model;
                return View(formData);
            }

            if (result.MatriculaID == -3)
            {
                var msgBloqueo = "El alumno tiene la MATRÍCULA CANCELADA y está bloqueado para matricularse. " +
                                 "Requiere autorización de un supervisor para levantar la condición.";
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = msgBloqueo });
                ModelState.AddModelError("Matricula.AlumnoID", msgBloqueo);
                var formData = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                formData.Matricula = model;
                return View(formData);
            }

            // Mensaje informativo: lista de espera y/o reingreso con número histórico
            var mensaje = "Matrícula registrada correctamente.";
            if (result.EnListaEspera)
                mensaje = "El curso no tiene cupos disponibles. La matrícula quedó en LISTA DE ESPERA.";
            if (!string.IsNullOrEmpty(result.NroMatriculaAnterior))
                mensaje += $" Reingreso: nuevo N° {result.NroMatricula} (N° anterior {result.NroMatriculaAnterior} conservado como histórico).";

            if (Request.IsAjaxRequest())
                return Json(new { success = true, enListaEspera = result.EnListaEspera, message = mensaje, redirectUrl = Url.Action("Ficha", "Alumnos", new { id = model.AlumnoID }) });
            return RedirectToAction("Ficha", "Alumnos", new { id = model.AlumnoID });
        }

        // GET: /Matricula/Editar/5
        public async Task<ActionResult> Editar(int? id)
        {
            if (id == null) return HttpNotFound();
            var matricula = await _matriculaService.GetMatriculaAsync(id.Value);
            if (matricula == null) return HttpNotFound();

            var formData = await _matriculaService.GetFormDataAsync(matricula.AlumnoID);
            formData.Matricula = matricula;
            return View("Crear", formData);
        }

        // POST: /Matricula/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(MatriculaFormViewModel form)
        {
            var model = form.Matricula;

            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                var formData = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                formData.Matricula = model;
                return View("Crear", formData);
            }

            var result = await _matriculaService.UpdateMatriculaAsync(model, UserEmail());

            if (result == -1)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "El alumno ya tiene una matrícula para ese año escolar." });
                ModelState.AddModelError("Matricula.AnioEscolarID",
                    "El alumno ya tiene una matrícula para ese año escolar.");
                var formData = await _matriculaService.GetFormDataAsync(model.AlumnoID);
                formData.Matricula = model;
                return View("Crear", formData);
            }

            if (Request.IsAjaxRequest())
                return Json(new { success = true, message = "Matrícula actualizada correctamente.", redirectUrl = Url.Action("Ficha", "Alumnos", new { id = model.AlumnoID }) });
            return RedirectToAction("Ficha", "Alumnos", new { id = model.AlumnoID });
        }

        // POST: /Matricula/Anular
        [HttpPost]
        public async Task<JsonResult> Anular(int matriculaId, string observacion)
        {
            var result = await _matriculaService.AnularMatriculaAsync(matriculaId, observacion, UserEmail());
            return Json(new { success = result > 0 });
        }
    }
}