using Gestionador.Helpers;
using Gestionador.Helpers.Enum;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Responses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class ReportService : BaseServices, IReportService
    {
        private protected readonly ISolicitudesService _solicitudesService;
        public ReportService(Entities dbContext, IMapperService mapperService, ISolicitudesService solicitudesService) : base(dbContext, mapperService)
        {
            _solicitudesService = solicitudesService;
        }

        public async Task<List<ConceptosReportResponse>> GetReportAndServiciosAsync(DateTime startDate, DateTime endDate)
        {
            List<ConceptosReportResponse> conceptosReports = new List<ConceptosReportResponse>();
            var dateResult = ReportHelper.ConvertToDateUtc(startDate, endDate);
            var adquisicionesTask = await _dbContext.tbl_Adquisicion
                .Where(a => a.FechaAdquisicion >= dateResult.Item1 && a.FechaAdquisicion <= dateResult.Item2)
                .Join(
                    _dbContext.tbl_Conceptos,
                    ad => ad.ProductoID,
                    co => co.ID,
                    (ad, co) => new { ad, co.Concepto, co.CategoriaID, ad.PrecioProducto, ad.FechaAdquisicion }
                )
                .Join(
                    _dbContext.tbl_Categorias,
                    co => co.CategoriaID,
                    ca => ca.ID,
                    (co, ca) => new { Categoria = ca.Categoria, Producto = co.Concepto, co.PrecioProducto, co.FechaAdquisicion }
                )
                .GroupBy(
                    g => new { g.Categoria, g.Producto, Fecha = DbFunctions.TruncateTime(g.FechaAdquisicion) }
                )
                .Select(g => new ConceptosReportResponse
                {
                    Concepto = g.Key.Producto,
                    Categoria = g.Key.Categoria,
                    FechaRegistro = g.Key.Fecha.Value,
                    Count = g.Count(),
                    Total = g.Sum(x => x.PrecioProducto)
                })
                .ToListAsync();

            var serviciosTask = await _dbContext.tbl_ServiciosAdquisicion
                .Where(sa => sa.FechaRegistro >= dateResult.Item1 && sa.FechaRegistro <= dateResult.Item2 && sa.IsActive)
                .Join(
                    _dbContext.tbl_Servicios,
                    sa => sa.ServicioID,
                    se => se.ID,
                    (sa, se) => new { sa, se.Servicio, se.CategoriaID, sa.Precio, sa.FechaRegistro }
                )
                .Join(
                    _dbContext.tbl_Categorias,
                    se => se.CategoriaID,
                    ca => ca.ID,
                    (se, ca) => new { Categoria = ca.Categoria, Servicio = se.Servicio, se.Precio, se.FechaRegistro }
                )
                .GroupBy(
                    g => new { g.Categoria, g.Servicio, Fecha = DbFunctions.TruncateTime(g.FechaRegistro) }
                )
                .Select(g => new ConceptosReportResponse
                {
                    Concepto = g.Key.Servicio,
                    Categoria = g.Key.Categoria,
                    FechaRegistro = g.Key.Fecha.Value,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Precio)
                }).ToListAsync();

            var mantencionesTask = await _dbContext.tbl_PagosMantenciones
                .Where(sa => sa.FechaRegistro >= dateResult.Item1 && sa.FechaRegistro <= dateResult.Item2 && sa.IsActive)
                .Join(
                    _dbContext.tbl_Servicios,
                    sa => sa.MantencionID,
                    se => se.ID,
                    (sa, se) => new { sa, se.Servicio, se.CategoriaID, sa.Precio, sa.FechaRegistro }
                )
                .Join(
                    _dbContext.tbl_Categorias,
                    se => se.CategoriaID,
                    ca => ca.ID,
                    (se, ca) => new { Categoria = ca.Categoria, Servicio = se.Servicio, se.Precio, se.FechaRegistro }
                )
                .GroupBy(
                    g => new { g.Categoria, g.Servicio, Fecha = DbFunctions.TruncateTime(g.FechaRegistro) }
                )
                .Select(g => new ConceptosReportResponse
                {
                    Concepto = g.Key.Servicio,
                    Categoria = g.Key.Categoria,
                    FechaRegistro = g.Key.Fecha.Value,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Precio)
                }).ToListAsync();

            conceptosReports.AddRange(adquisicionesTask);
            conceptosReports.AddRange(serviciosTask);
            conceptosReports.AddRange(mantencionesTask);

            return conceptosReports;
        }

        public async Task<List<ReportVentasResponse>> GetReportVentasAsync(DateTime startDate, DateTime endDate, IEnumerable<int> formasPagoIds = null)
        {
            var dateResult = ReportHelper.ConvertToDateUtc(startDate, endDate);

            var query = _dbContext.tbl_PagoAdquisicion
                        .Where(a => a.FechaRegistro >= dateResult.Item1 && a.FechaRegistro <= dateResult.Item2 && a.IsActive)
                        .Join(_dbContext.tbl_FormasPago, pa => pa.FormaPagoID, fp => fp.ID, (pa, fp) => new { pa, fp });

            if (formasPagoIds != null && formasPagoIds.Any())
            {
                query = query.Where(x => formasPagoIds.Contains(x.fp.ID));
            }

            var result = await query
                        .GroupBy(g => new { g.fp.FormaPago, Fecha = DbFunctions.TruncateTime(g.pa.FechaRegistro) })
                        .Select(g => new ReportVentasResponse
                        {
                            FormaPago = g.Key.FormaPago,
                            FechaRegistro = g.Key.Fecha.Value,
                            Total = g.Sum(x => x.pa.Monto)
                        }).ToListAsync();

            return result;
        }

        public async Task<List<AdquisicionResponse>> GetReportDetalleAsync(DateTime startDate, DateTime endDate, IEnumerable<int> ConceptoId, bool onlyRegulated)
        {
            var dateResult = ReportHelper.ConvertToDateUtc(startDate, endDate);

            var query = await _dbContext.tbl_Adquisicion
                .Include(x => x.tbl_Cementerios)
                .Include(x => x.tbl_Estados)
                .Where(a => a.FechaAdquisicion >= dateResult.Item1 && a.FechaAdquisicion <= dateResult.Item2 && ConceptoId.Contains(a.ProductoID) && a.IsRegulated == onlyRegulated)
                .ToListAsync();                        

            return query.Select(s => _mapperService.MapToAdquisicionResponse(s)).ToList();
        }

        public async Task<List<ReportCobranzaResponse>> GetReportCobranzaAsync(DateTime startDate, DateTime endDate)
        {
            List<int> pendientes = new List<int> { (int)StatusType.PagoParcial, (int)StatusType.Impaga };
            var dateResult = ReportHelper.ConvertToDateUtc(startDate, endDate);

            var agrupados = await _dbContext.tbl_Adquisicion
                 .Include(x => x.tbl_ServiciosAdquisicion)
                 .Where(x => x.FechaAdquisicion >= dateResult.Item1 && x.FechaAdquisicion <= dateResult.Item2 && pendientes.Contains(x.EstadoID))
                 .GroupBy(x => new
                 {
                     x.ID,
                     x.NroFicha,
                     x.CompRut,
                     x.CompNombre,
                     x.CompApellido,
                     x.CompTelefono,
                     x.tbl_Comunas.Comuna,
                     x.UbiNumero,
                     x.LetraNicho,
                     x.tbl_Secciones.Seccion,
                     x.tbl_Secciones.tbl_Patios.Patio,
                     x.PrecioProducto
                 })
                 .ToListAsync();

            var resultado = agrupados.Select(s => new ReportCobranzaResponse
            {
                ID = s.Key.ID,
                NroFicha = s.Key.NroFicha,
                CompRut = s.Key.CompRut,
                CompNombre = s.Key.CompNombre,
                CompApellido = s.Key.CompApellido,
                CompComuna = s.Key.Comuna,
                CompTelefono = s.Key.CompTelefono,
                UbiNumero = s.Key.UbiNumero,
                LetraNicho = s.Key.LetraNicho,
                Sector = s.Key.Seccion,
                Patio = s.Key.Patio.ToString(),
                Deuda = Math.Abs(s.Sum(x => x.tbl_PagoAdquisicion.Where(w => w.IsActive).Sum(a => a.Monto)) -
                        (s.Key.PrecioProducto +
                         s.Sum(x => x.tbl_PagosMantenciones.Where(w => w.IsActive).Sum(a => a.Precio)) +
                         s.Sum(x => x.tbl_ServiciosAdquisicion.Where(w => w.IsActive).Sum(a => a.Precio))))
            }).ToList();

            return resultado;
        }
    }
}