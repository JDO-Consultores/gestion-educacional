using Gestionador.Interfaces;
using Gestionador.Models;
using Gestionador.Responses;
using KendoNET.DynamicLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize]
    public class SolicitudesController : DefaultController
    {
        private readonly ISearchService _searchService;
        private readonly ISolicitudesService _solicitudesService;
        public SolicitudesController(ISearchService searchService, ISolicitudesService solicitudesService)
        {
            _searchService = searchService;
            _solicitudesService = solicitudesService;
        }

        public ActionResult Index()
        {
            ViewBag.IsAdmin = IsAdmin();
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Cementerios = _searchService.GetCementerios();
            ViewBag.regiones = _searchService.GetRegiones();
            ViewBag.servicios = _searchService.GetServiciosById(new int[] { 2 });
            ViewBag.formasPago = _searchService.GetFormasPago();
            ViewBag.letras = _searchService.GetLetrasNichos();
            return View();
        }

        public async Task<JsonResult> GetSolicitudesIndexAsync(DataSourceRequest request)
        {
            dynamic solicitudes = await _solicitudesService.GetSolicitudesIndexAsync(request);
            return Json(solicitudes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> EditSolicitud(int id)
        {
            var userId = UserId();
            var adquisicion = await _solicitudesService.GetSolicitudById(id);
            ViewBag.regiones = _searchService.GetRegiones();
            ViewBag.formasPago = _searchService.GetFormasPago();
            return View(adquisicion);
        }

        [HttpGet]
        public async Task<ActionResult> ViewSolicitud(int id)
        {
            var userId = UserId();
            var adquisicion = await _solicitudesService.GetSolicitudById(id);
            ViewBag.regiones = _searchService.GetRegiones();
            ViewBag.formasPago = _searchService.GetFormasPago();
            return View(adquisicion);
        }

        [HttpGet]
        public async Task<ActionResult> _TransferAdquisicion(int id)
        {
            ViewBag.regiones = _searchService.GetRegiones();
            var model = new TransferAdquisicionViewModel
            {
                ID = id
            };
            return PartialView(model);
        }

        [HttpGet]
        public async Task<ActionResult> _AnularAdquisicion(int id)
        {
            ViewBag.Id = id;
            return PartialView();
        }

        [HttpGet]
        public ActionResult BusquedasAdquisicion()
        {
            ViewBag.Cementerios = _searchService.GetCementerios();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> _GenerarPdf(int id)
        {
            ViewBag.Parrocos = _searchService.GetParrocos();
            var model = new AdquisicionResponse
            {
                ID = id
            };
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> BuscarAdquisicionAsync(string dropdownValue, string searchText)
        {
            if (string.IsNullOrEmpty(dropdownValue) || string.IsNullOrEmpty(searchText))
            {
                return Json(new { success = false, message = "Ingrese el valor a encontrar." }, JsonRequestBehavior.AllowGet);
            }

            var result = await _solicitudesService.BuscarAdquisicionAsync(dropdownValue, searchText);

            if (result.Count == 0)
            {
                return Json(new { success = false, message = "No se encontraron coincidencias." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateAsync(FichaViewModelBase model, string DifuntosJson, string ServiciosJson, string FormasPagoJson)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(DifuntosJson))
            {
                model.Difuntos = JsonConvert.DeserializeObject<List<DifuntoViewModel>>(DifuntosJson);

                for (int i = 0; i < model.Difuntos.Count; i++)
                {
                    var fileKey = $"difuntoFiles[{i}]";
                    if (Request.Files[fileKey] != null && Request.Files[fileKey].ContentLength > 0)
                    {
                        var file = Request.Files[fileKey];
                        var fileName = Path.GetFileName(file.FileName);

                        var uploadPath = Server.MapPath("~/Uploads/Ordenes");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        file.SaveAs(filePath);
                        model.Difuntos[i].AdjuntarOrden = $"~/Uploads/Ordenes/{uniqueFileName}";
                    }
                }
            }
            if (!string.IsNullOrEmpty(ServiciosJson))
            {
                model.Servicios = JsonConvert.DeserializeObject<List<ServicioViewModel>>(ServiciosJson);
            }
            if (!string.IsNullOrEmpty(FormasPagoJson))
            {
                model.FormaPagos = JsonConvert.DeserializeObject<List<FormaPagoViewModel>>(FormasPagoJson);
            }

            var result = await _solicitudesService.CreateAdquisicionAsync(UserId(), model);

            if (result == -1)
            {
                return Json(new { success = false, message = "No hay stock disponible para el sector seleccionado." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -2)
            {
                return Json(new { success = false, message = "El pago es mayor a los productos/servicios." }, JsonRequestBehavior.AllowGet);
            }
            else if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente.", redirectUrl = Url.Action("EditSolicitud", new { id = result }) }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -4)
            {
                return Json(new { success = false, message = "Este derecho de adquisición ya fue vendida." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -5)
            {
                return Json(new { success = false, message = "No se ha ingresado la moneda actualizada." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> TransferAdquisicionAsync(TransferAdquisicionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _solicitudesService.TransferAdquisicionAsync(UserId(), model);

            if (result == -1)
            {
                return Json(new { success = false, message = "Esta adquisición ya fue transferida." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -2)
            {
                return Json(new { success = false, message = "La adquisición debe tener sus pagos completos." }, JsonRequestBehavior.AllowGet);
            }
            else if (result > 0)
            {
                return Json(new { success = true, message = "Adquisición Transferida Correctamente.", redirectUrl = Url.Action("EditSolicitud", new { id = result }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Transferencia fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ImprimirPdf(int id, int parrocoId, string checkStates)
        {
            var selectedChecks = JsonConvert.DeserializeObject<List<Models.CheckState>>(checkStates);

            var pdfBytes = await _solicitudesService.GenerateReport(id, parrocoId, UserId(), selectedChecks);
            return File(pdfBytes, "application/pdf", $"{DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH:mm")}-Adquisición.pdf");
        }

        [HttpPost]
        public async Task<ActionResult> AnularFicha(int id)
        {
            var result = await _solicitudesService.AnularFichaAsync(UserId(), id);

            if (result)
            {
                return Json(new { success = false, message = "Ficha anulada correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "La ficha ya se encuentra anulada." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("Solicitudes/EditAsync/{ID}")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditAsync(int ID, FichaViewModel model, string DifuntosJson, string ServiciosJson, string FormasPagoJson, string MantencionJson)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message = errors }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(DifuntosJson))
            {
                model.Difuntos = JsonConvert.DeserializeObject<List<DifuntoViewModel>>(DifuntosJson);
            }
            if (!string.IsNullOrEmpty(ServiciosJson))
            {
                model.Servicios = JsonConvert.DeserializeObject<List<ServicioViewModel>>(ServiciosJson);
            }
            if (!string.IsNullOrEmpty(FormasPagoJson))
            {
                model.FormaPagos = JsonConvert.DeserializeObject<List<FormaPagoViewModel>>(FormasPagoJson);
            }
            if (!string.IsNullOrEmpty(FormasPagoJson))
            {
                model.Mantenciones = JsonConvert.DeserializeObject<List<MantencionViewModel>>(MantencionJson);
            }
            var result = await _solicitudesService.EditAdquisicionAsync(UserId(), ID, model);

            if (result == -2)
            {
                return Json(new { success = false, message = "El pago es mayor a los productos/servicios." }, JsonRequestBehavior.AllowGet);
            }
            else if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente.", redirectUrl = Url.Action("EditSolicitud", new { id = result }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Creación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}