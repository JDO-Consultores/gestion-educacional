using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class AlumnosController : DefaultController
    {
        private readonly IAlumnosServices _alumnosServices;

        public AlumnosController(IAlumnosServices alumnosServices)
        {
            _alumnosServices = alumnosServices;
        }

        // GET: /Alumnos
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin      = IsAdmin();
            ViewBag.EstadosAlumno = await _alumnosServices.GetEstadosAlumnoAsync();
            ViewBag.AnioActivo = await _alumnosServices.GetAnioActivoAsync();
            return View();
        }

        // GET: /Alumnos/GetEstadosAlumno
        [HttpGet]
        public async Task<JsonResult> GetEstadosAlumno()
        {
            var estados = await _alumnosServices.GetEstadosAlumnoAsync();
            var result  = estados.Select(e => new { EstadoAlumno = e.Texto, ID = e.ID });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Alumnos/GetAniosAlumnos
        [HttpGet]
        public async Task<JsonResult> GetAniosAlumnos()
        {
            var anios  = await _alumnosServices.GetAniosEscolaresAlumnosAsync();
            var result = anios.Select(a => new { AnioEscolar = a.ToString(), ID = a });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /Alumnos/GetAlumnosIndexAsync
        [HttpPost]
        public async Task<JsonResult> GetAlumnosIndexAsync(DataSourceRequest request)
        {
            var result = await _alumnosServices.GetAlumnosIndexAsync(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: /Alumnos/Ficha/5
        public async Task<ActionResult> Ficha(int id)
        {
            var ficha = await _alumnosServices.GetFichaAlumnoAsync(id);
            if (ficha == null) return HttpNotFound();
            return View(ficha);
        }

        // GET: /Alumnos/Crear
        public ActionResult Crear()
        {
            return View(new AlumnoFichaViewModel());
        }

        // POST: /Alumnos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(AlumnoFichaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                return View(model);
            }
            try
            {
                var id = await _alumnosServices.CreateAlumnoAsync(model, GetCurrentUsername());
                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Alumno registrado correctamente.", redirectUrl = Url.Action("Ficha", new { id }) });
                return RedirectToAction("Ficha", new { id });
            }
            catch (InvalidOperationException ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = ex.Message });
                ModelState.AddModelError("Rut", ex.Message);
                return View(model);
            }
        }

        // GET: /Alumnos/Editar/5
        public async Task<ActionResult> Editar(int id)
        {
            var ficha = await _alumnosServices.GetFichaAlumnoAsync(id);
            if (ficha == null) return HttpNotFound();
            return View(ficha);
        }

        // POST: /Alumnos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(AlumnoFichaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Datos inválidos. Revise los campos requeridos." });
                return View(model);
            }
            try
            {
                await _alumnosServices.UpdateAlumnoAsync(model, GetCurrentUsername());
                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Alumno actualizado correctamente.", redirectUrl = Url.Action("Ficha", new { id = model.ID }) });
                return RedirectToAction("Ficha", new { id = model.ID });
            }
            catch (InvalidOperationException ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = ex.Message });
                ModelState.AddModelError("Rut", ex.Message);
                return View(model);
            }
            return RedirectToAction("Ficha", new { id = model.ID });
        }

        // POST: /Alumnos/Retirar
        [HttpPost]
        public async Task<JsonResult> Retirar(int alumnoId, int causalRetiroId, string fechaRetiro, string observacion)
        {
            DateTime fecha;
            if (!DateTime.TryParseExact(fechaRetiro, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out fecha))
            {
                fecha = DateTime.Today;
            }

            var result = await _alumnosServices.RetirarAlumnoAsync(alumnoId, causalRetiroId, fecha, observacion, GetCurrentUsername());
            return Json(new { success = result > 0 });
        }

        // POST: /Alumnos/CambioRut
        [HttpPost]
        public async Task<JsonResult> CambioRut(CambioRutViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RutNuevo))
                return Json(new { success = false, error = "Debe ingresar el RUT nuevo." });

            var (nuevoId, error) = await _alumnosServices.CambioRutAsync(model, GetCurrentUsername());
            if (!string.IsNullOrEmpty(error))
                return Json(new { success = false, error });

            return Json(new { success = true, nuevoAlumnoId = nuevoId });
        }

        // GET: /Alumnos/ValidarRut?rut=12345678-9&alumnoId=0
        [HttpGet]
        public async Task<JsonResult> ValidarRut(string rut, int alumnoId = 0)
        {
            var duplicado = await _alumnosServices.RutExisteAsync(rut, alumnoId);
            return Json(new { duplicado }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Alumnos/GetHistorial?alumnoId=5
        [HttpGet]
        public async Task<JsonResult> GetHistorial(int alumnoId)
        {
            var historial = await _alumnosServices.GetHistorialAsync(alumnoId);
            return Json(historial, JsonRequestBehavior.AllowGet);
        }

        // ─── Alergias ──────────────────────────────────────────────────────────

        // GET: /Alumnos/GetAlergias?alumnoId=5
        [HttpGet]
        public async Task<JsonResult> GetAlergias(int alumnoId)
        {
            var alergias = await _alumnosServices.GetAlergiasAlumnoAsync(alumnoId);
            return Json(alergias, JsonRequestBehavior.AllowGet);
        }

        // POST: /Alumnos/GuardarAlergia
        [HttpPost]
        public async Task<JsonResult> GuardarAlergia(AlumnoAlergiaViewModel model)
        {
            // TipoAlergiaID 1 = Informativa -> requiere certificado en alta
            if (model.TipoAlergiaID == 1 && model.ID == 0 &&
                (Request.Files["CertificadoArchivo"] == null || Request.Files["CertificadoArchivo"].ContentLength == 0))
            {
                return Json(new { success = false, message = "Las alergias Informativas requieren un certificado." });
            }

            if (model.TipoAlergiaID == 2 &&
                string.Equals(model.NombreAlergia, "Otros", System.StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrWhiteSpace(model.Descripcion))
            {
                return Json(new { success = false, message = "Debe ingresar el detalle de la alergia alimenticia." });
            }

            if (Request.Files["CertificadoArchivo"] != null && Request.Files["CertificadoArchivo"].ContentLength > 0)
                model.CertificadoArchivo = Request.Files["CertificadoArchivo"];

            var id = await _alumnosServices.GuardarAlergiaAsync(model, GetCurrentUsername());
            return Json(new { success = id > 0, message = id > 0 ? "Alergia guardada correctamente." : "Error al guardar la alergia." });
        }

        // POST: /Alumnos/EliminarAlergia
        [HttpPost]
        public async Task<JsonResult> EliminarAlergia(int alergiaId)
        {
            var result = await _alumnosServices.EliminarAlergiaAsync(alergiaId, GetCurrentUsername());
            return Json(new { success = result > 0 });
        }

        // GET: /Alumnos/DescargarCertificadoAlergia/5
        [HttpGet]
        public async Task<ActionResult> DescargarCertificadoAlergia(int id)
        {
            var (contenido, nombre, mimeType) = await _alumnosServices.DescargarCertificadoAlergiaAsync(id);
            if (contenido == null) return HttpNotFound();
            return File(contenido, mimeType ?? "application/octet-stream", nombre);
        }

        // ─── Discapacidades ──────────────────────────────────────────────────

        // GET: /Alumnos/GetDiscapacidades?alumnoId=5
        [HttpGet]
        public async Task<JsonResult> GetDiscapacidades(int alumnoId)
        {
            var lista = await _alumnosServices.GetDiscapacidadesAlumnoAsync(alumnoId);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // POST: /Alumnos/EliminarDiscapacidad
        [HttpPost]
        public async Task<JsonResult> EliminarDiscapacidad(int discapacidadId)
        {
            var result = await _alumnosServices.EliminarDiscapacidadAsync(discapacidadId, GetCurrentUsername());
            return Json(new { success = result > 0 });
        }

        // GET: /Alumnos/DescargarCertificadoDiscapacidad/5
        [HttpGet]
        public async Task<ActionResult> DescargarCertificadoDiscapacidad(int id)
        {
            var (contenido, nombre, mimeType) = await _alumnosServices.DescargarCertificadoDiscapacidadAsync(id);
            if (contenido == null) return HttpNotFound();
            return File(contenido, mimeType ?? "application/octet-stream", nombre);
        }
    }
}