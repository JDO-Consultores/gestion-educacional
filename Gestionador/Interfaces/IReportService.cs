using Gestionador.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Gestionador.Interfaces
{
    public interface IReportService
    {
        Task<List<ConceptosReportResponse>> GetReportAndServiciosAsync(DateTime startDate, DateTime endDate);
        Task<List<ReportVentasResponse>> GetReportVentasAsync(DateTime startDate, DateTime endDate, IEnumerable<int> formasPagoIds = null);
        Task<List<AdquisicionResponse>> GetReportDetalleAsync(DateTime startDate, DateTime endDate, IEnumerable<int> ConceptoId, bool onlyRegulated);
        Task<List<ReportCobranzaResponse>> GetReportCobranzaAsync(DateTime startDate, DateTime endDate);
    }
}