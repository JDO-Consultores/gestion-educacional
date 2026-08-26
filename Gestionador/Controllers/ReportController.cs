using Gestionador.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportController : DefaultController
    {
        private readonly IReportService _reportService;
        private readonly ISearchService _searchService;
        public ReportController(IReportService reportService , ISearchService searchService)
        {
            _reportService = reportService;
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult> Report()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReportVentas()
        {
            ViewBag.FormasPago = _searchService.GetFormasPago();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReportDetalle()
        {
            ViewBag.Conceptos = _searchService.GetConceptosAsync();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ReportCobranza()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetReportStarEndDate(DateTime startDate, DateTime endDate)
        {
            var result = await _reportService.GetReportAndServiciosAsync(startDate, endDate);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public async Task<JsonResult> GetReportVentas(DateTime startDate, DateTime endDate, IEnumerable<int> formasPagoIds)
        {
            var result = await _reportService.GetReportVentasAsync(startDate, endDate, formasPagoIds);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetReportDetalle(DateTime startDate, DateTime endDate, IEnumerable<int> conceptoId, bool onlyRegulated)
        {
            if (conceptoId == null || !conceptoId.Any())
            {
                return Json(new { success = false, message = "Debe seleccionar al menos un producto." }, JsonRequestBehavior.AllowGet);
            }

            var result = await _reportService.GetReportDetalleAsync(startDate, endDate, conceptoId, onlyRegulated);
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetReportCobranza(DateTime startDate, DateTime endDate)
        {
            var result = await _reportService.GetReportCobranzaAsync(startDate, endDate);
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}