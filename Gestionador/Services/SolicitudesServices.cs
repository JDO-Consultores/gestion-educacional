using AutoMapper;
using Gestionador.Helpers;
using Gestionador.Helpers.Enum;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using KendoNET.DynamicLinq;
using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class SolicitudesServices : BaseServices, ISolicitudesService
    {
        private SearchService _searchService;
        private readonly IMapper _mapper;
        private readonly IUserService _userServices;
        public SolicitudesServices(Entities dbContext, IMapperService mapperService, SearchService searchService, IMapper mapper, IUserService userService) : base(dbContext, mapperService)
        {
            _searchService = searchService;
            _mapper = mapper;
            _userServices = userService;
        }

        public async Task<DataSourceResult> GetSolicitudesIndexAsync(DataSourceRequest request)
        {
            var query = await GetAdquisicionBaseQueryAsync().ToListAsync();
            var mappedData = query.Select(s => _mapperService.MapToAdquisicionBaseResponse(s)).AsQueryable();
            var result = await mappedData.OrderByDescending(x => x.ID)
                               .ToDataSourceResultAsync(request.Take, request.Skip, request.Sort, request.Filter, request.Aggregate, request.Group);

            return result;
        }

        public async Task<AdquisicionResponse> GetSolicitudById(int id)
        {
            return _mapperService.MapToAdquisicionResponse(await GetAdquisicionDbByIdAsync(id));
        }

        public async Task<List<AdquisicionBase>> BuscarAdquisicionAsync(string dropdownValue, string searchText)
        {
            List<tbl_Adquisicion> adquisicion = new List<tbl_Adquisicion>();
            switch (dropdownValue)
            {
                case "Número Ficha":
                    adquisicion = await GetAdquisicionBaseQueryAsync(x => x.NroFicha.Equals(searchText)).ToListAsync();
                    break;
                case "Rut":
                    string rut = ValidaRut.Parse(searchText);
                    adquisicion = await GetAdquisicionBaseQueryAsync(x => x.CompRut.Contains(searchText) || x.RefRut.Contains(searchText)).ToListAsync();
                    break;
                case "NOMBRE COMPRADOR/REFERENTE":
                    adquisicion = await GetAdquisicionBaseQueryAsync(x => x.CompNombre.Contains(searchText) || x.RefNombre.Contains(searchText)
                                                || x.CompApellido.Contains(searchText) || x.RefApellido.Contains(searchText) ||
                                                (x.CompNombre + " " + x.CompApellido).Contains(searchText) ||
                                                (x.RefNombre + " " + x.RefApellido).Contains(searchText)).ToListAsync();
                    break;
                case "Fecha Adquisición":
                    var formatDate = DateTime.Parse(searchText);
                    var startDate = formatDate.Date.ToUniversalTime();
                    var endDate = startDate.AddDays(1).AddTicks(-1).ToUniversalTime();
                    adquisicion = await GetAdquisicionBaseQueryAsync(x => x.FechaAdquisicion >= startDate && x.FechaAdquisicion <= endDate).ToListAsync();
                    break;
                case "Difunto":
                    adquisicion = await GetAdquisicionBaseQueryAsync(x => x.tbl_Difuntos.Any(w => w.NombreApellido.Contains(searchText))).ToListAsync(); 
                    break;
                default:
                    break;
            }
            return adquisicion.Select(s => _mapperService.MapToAdquisicionBaseResponse(s)).OrderByDescending(w => w.ID).ToList();
        }

        public async Task<int> CreateAdquisicionAsync(int userId, FichaViewModelBase model)
        {
            var conceptoSector = _searchService.GetSeccionConceptoById(model.ProductoID);
            if (conceptoSector.Stock == 0 && !model.IsRegulated)
            {
                return -1;
            }

            if (model.IsRegulated)
            {
                conceptoSector.Precio = 0;
                model.Servicios.Clear();
                model.FormaPagos.Clear();
            }

            if (await GetAdquisicionBaseQueryAsync(x => x.ProductoID == conceptoSector.ConceptoID && x.UbiSectorID == model.UbiSector && x.UbiNumero.Equals(model.UbiNumero)).AnyAsync())
            {
                return -4;
            }

            var estadoId = CalculateEstado(model.Servicios, model.Mantenciones, model.FormaPagos, conceptoSector);
            if (estadoId == -1)
            {
                return -2;
            }

            if (!model.IsRegulated)
            {
                await DiscountStock(conceptoSector.ID);
            }

            tbl_Adquisicion adquisicion = new tbl_Adquisicion
            {
                NroFicha = string.Empty,
                CementerioID = model.CementerioID,
                UbiSectorID = model.UbiSector,
                UbiNumero = model.UbiNumero,
                ProductoID = conceptoSector.ConceptoID,
                Producto = conceptoSector.Concepto.Concepto.ToUpper(),
                PrecioProducto = conceptoSector.Precio,
                FechaAdquisicion = DateTime.UtcNow,
                UsuarioID = userId,
                EstadoID = estadoId,
                IsRegulated = model.IsRegulated,
                IsTrasferred = false,
                LetraNicho = model.Letra,
                TipoMonedaID = conceptoSector.TipoMonedaID              
            };
            var valorMoneda = await _searchService.ValorMonedaMesActualAsync(conceptoSector.TipoMonedaID);

            if (valorMoneda.Item1 == -1)
            {
                return -5;
            }
            adquisicion.PrecioProducto = conceptoSector.Precio * valorMoneda.Item1;
            adquisicion.PrecioTipoMoneda = conceptoSector.Precio;
            adquisicion.ValorTipoMonedaActual = valorMoneda.Item1;
            adquisicion.FechaTipoMoneda = valorMoneda.Item2;

            UpdateComprador(adquisicion, model.Comprador, model.Referente);
            _dbContext.tbl_Adquisicion.Add(adquisicion);

            if (model.Difuntos?.Any() ?? false)
            {
                foreach (var item in model.Difuntos)
                {
                    await AddDifuntosAsync(adquisicion, userId, item);
                }
            }

            if (model.Servicios?.Any() ?? false && !model.IsRegulated)
            {
                foreach (var item in model.Servicios)
                {
                    await AddServiciosAsync(adquisicion, userId, item);
                }
            }

            if (model.FormaPagos?.Any() ?? false && !model.IsRegulated)
            {
                foreach (var item in model.FormaPagos)
                {
                    await AddFormaPagoAsync(adquisicion, userId, item);
                }
            }

            await AddObservacion(adquisicion.ID, model.Observacion, userId);
            await AddHistorial(adquisicion.ID, "Ha creado una adquisición", userId);
            await _dbContext.SaveChangesAsync();
            adquisicion.NroFicha = await GetNextNroFichaAsync(adquisicion.CementerioID, adquisicion.IsRegulated);
            await _dbContext.SaveChangesAsync();
            return adquisicion.ID;
        }

        public async Task<int> EditAdquisicionAsync(int userId, int id, FichaViewModel model)
        {
            var adquisicion = _dbContext.tbl_Adquisicion.Find(id);
            var conceptoSector = _searchService.GetSeccionConceptoById(model.ProductoID);
            if (adquisicion != null)
            {
                UpdateComprador(adquisicion, model.Comprador, model.Referente);

                conceptoSector.Precio = adquisicion.PrecioProducto;
                adquisicion.EstadoID = CalculateEstado(model.Servicios, model.Mantenciones, model.FormaPagos, conceptoSector);
            }
            if (adquisicion.EstadoID == -1)
            {
                return -2;
            }

            if (model.Difuntos?.Any() ?? false)
            {
                adquisicion.tbl_Difuntos.Where(w => w.ID != 0 && !model.Difuntos.Any(a => a.ID == w.ID)).ForEach(f => { f.IsActive = false; });

                foreach (var item in model.Difuntos)
                {
                    await AddDifuntosAsync(adquisicion, userId, item);
                }
            }

            if (model.Servicios?.Any() ?? false)
            {
                adquisicion.tbl_ServiciosAdquisicion.Where(w => w.ID != 0 && !model.Servicios.Any(a => a.ID == w.ID)).ForEach(f => f.IsActive = false);
                foreach (var item in model.Servicios)
                {
                    await AddServiciosAsync(adquisicion, userId, item);
                }
            }

            if (model.Mantenciones?.Any() ?? false)
            {
                adquisicion.tbl_PagosMantenciones.Where(w => w.ID != 0 && !model.Mantenciones.Any(a => a.ID == w.ID)).ForEach(f => f.IsActive = false);
                foreach (var item in model.Mantenciones)
                {
                    await AddMantencionesAsync(adquisicion, userId, item);
                }
            }

            if (model.FormaPagos?.Any() ?? false)
            {
                adquisicion.tbl_PagoAdquisicion.Where(w => w.ID != 0 && !model.FormaPagos.Any(a => a.ID == w.ID)).ForEach(f => f.IsActive = false);
                foreach (var item in model.FormaPagos)
                {
                    await AddFormaPagoAsync(adquisicion, userId, item);
                }
            }

            await AddObservacion(adquisicion.ID, model.Observacion, userId);
            await AddHistorial(adquisicion.ID, "Ha modificado una adquisición", userId);
            _dbContext.Entry(adquisicion).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
            return adquisicion.ID;
        }

        public async Task<int> TransferAdquisicionAsync(int userId, TransferAdquisicionViewModel model)
        {
            var adquisicion = await GetAdquisicionDbByIdAsync(model.ID);
            if (adquisicion.IsTrasferred)
            {
                return -1;
            }
            else if (adquisicion.EstadoID != 3)
            {
                return -2;
            }

            var nuevaAdquisicion = _mapper.Map<tbl_Adquisicion>(adquisicion);
            UpdateComprador(nuevaAdquisicion, model.Comprador, model.Referente);

            nuevaAdquisicion.EstadoID = 3;
            nuevaAdquisicion.PrecioProducto = 0;
            nuevaAdquisicion.UsuarioID = userId;
            nuevaAdquisicion.FechaAdquisicion = DateTime.UtcNow;
            nuevaAdquisicion.IsTrasferred = false;
            nuevaAdquisicion.TranferredID = adquisicion.ID;
            nuevaAdquisicion.TransferredNroFicha = adquisicion.NroFicha;
            nuevaAdquisicion.IsRegulated = false;

            _dbContext.tbl_Adquisicion.Add(nuevaAdquisicion);
            await AddHistorial(nuevaAdquisicion.ID, $"Ha creado una adquisición desde un traspaso Ficha N° {adquisicion.NroFicha}", userId);
            await _dbContext.SaveChangesAsync();

            adquisicion.IsTrasferred = true;
            adquisicion.NextTransferID = nuevaAdquisicion.ID;
            adquisicion.NextTransferNroFicha = nuevaAdquisicion.NroFicha;
            await AddHistorial(adquisicion.ID, $"Traspaso una adquisición a la Ficha N° {nuevaAdquisicion.NroFicha}", userId);

            nuevaAdquisicion.NroFicha = await GetNextNroFichaAsync(adquisicion.CementerioID, false);
            await _dbContext.SaveChangesAsync();

            return nuevaAdquisicion.ID;
        }

        public async Task<byte[]> GenerateReport(int id, int parrocoId, int userId, List<CheckState> selectedChecks)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-CL");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-CL");

            var adquisicion = new List<AdquisicionResponse> { await GetSolicitudById(id) };
            var recaudador = await _userServices.GetUserByID(userId);
            var parroco = await _searchService.GetParrocoById(parrocoId);
            ReportViewer rpAdquisicion = new ReportViewer();

            rpAdquisicion.LocalReport.DataSources.Clear();
            rpAdquisicion.LocalReport.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["reportUrl"]);
            rpAdquisicion.LocalReport.EnableExternalImages = true;

            rpAdquisicion.LocalReport.DataSources.Add(new ReportDataSource("AdquisicionDataSet", adquisicion));
            rpAdquisicion.LocalReport.SubreportProcessing += (sender, e) =>
            {
                ConfigReportSubReportProcessing(sender, e, adquisicion);
            };
            var image = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["reportImage"])).AbsoluteUri;
            var qR = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["reportQr"])).AbsoluteUri;

            var parametros = new List<ReportParameter>
            {
                new ReportParameter("Recaudador", recaudador.NombreApellido.ToUpper()),
                new ReportParameter("NombreParroco", $"{parroco.TipoAdministrador.TipoAdministrador.ToUpper()} {parroco.NombreParroco.ToUpper()}" ),
                new ReportParameter("RutParroco", parroco.RutParroco),
                new ReportParameter("SaldoPendiente", CalculateSaldoPendiente(adquisicion[0].Servicios, adquisicion[0].Mantenciones, adquisicion[0].FormaPagos, adquisicion[0].PrecioProducto).ToString()),
                new ReportParameter("RutaImagen", image),
                new ReportParameter("RutaQr", qR),
                new ReportParameter("ReportFooterLinea1", ConfigurationManager.AppSettings["reportFooterLinea1"]),
                new ReportParameter("ReportFooterLinea2", ConfigurationManager.AppSettings["reportFooterLinea2"]),
            };

            foreach (var check in selectedChecks)
            {
                parametros.Add(new ReportParameter(check.Value, check.Checked.ToString()));
            }

            rpAdquisicion.LocalReport.SetParameters(parametros);
            rpAdquisicion.Refresh();
            byte[] Bytes = rpAdquisicion.LocalReport.Render(format: "PDF", deviceInfo: "");
            return Bytes;
        }

        public async Task<bool> AnularFichaAsync(int userId, int id)
        {
            var adquisicion = await _dbContext.tbl_Adquisicion.FindAsync(id);

            if (adquisicion.EstadoID == (int)StatusType.Anulada)
            {
                return false;
            }
            var conceptoSector = _searchService.GetSeccionConceptoBySeccionConceptoId(adquisicion.UbiSectorID, adquisicion.ProductoID);
            adquisicion.EstadoID = (int)StatusType.Anulada;

            if (!adquisicion.IsRegulated)
            {
                await IncreaseStock(conceptoSector.ID);
            }

            await AddHistorial(adquisicion.ID, $"Ha anulado la ficha", userId);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private void ConfigReportSubReportProcessing(object sender, SubreportProcessingEventArgs e, List<AdquisicionResponse> adquisicion)
        {
            ReportDataSource dp = new ReportDataSource("PersonaDataSet", new List<PersonaResponse> { adquisicion.FirstOrDefault().Comprador });
            e.DataSources.Add(dp);

            ReportDataSource dr = new ReportDataSource("ReferenteDataSet", new List<PersonaResponse> { adquisicion.FirstOrDefault().Referente });
            e.DataSources.Add(dr);

            ReportDataSource df = new ReportDataSource("DifuntoDataSet", adquisicion.FirstOrDefault().Difuntos.ToList());
            e.DataSources.Add(df);

            ReportDataSource ds = new ReportDataSource("ServicioDataSet", adquisicion.FirstOrDefault().Servicios.ToList());
            e.DataSources.Add(ds);

            ReportDataSource dfp = new ReportDataSource("PagoDataSet", adquisicion.FirstOrDefault().FormaPagos.ToList());
            e.DataSources.Add(dfp);

            ReportDataSource dfo = new ReportDataSource("ObservacionDataSet", adquisicion.FirstOrDefault().Observaciones.ToList());
            e.DataSources.Add(dfo);

            ReportDataSource dfm = new ReportDataSource("MantencionesDataSet", adquisicion.FirstOrDefault().Mantenciones.ToList());
            e.DataSources.Add(dfm);

            ReportDataSource tpm = new ReportDataSource("TipoMonedaDataSet", new List<TipoMonedaResponse> { adquisicion.FirstOrDefault().TipoMoneda });
            e.DataSources.Add(tpm);
        }

        private async Task AddDifuntosAsync(tbl_Adquisicion adquisicion, int userId, DifuntoViewModel difuntos)
        {
            tbl_Difuntos tbl_Difuntos = difuntos.ID == 0 ? new tbl_Difuntos
            {
                AdquisicionID = adquisicion.ID,
                UsuarioID = userId,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            } : adquisicion.tbl_Difuntos.SingleOrDefault(w => w.ID == difuntos.ID);

            tbl_Difuntos.CausaID = difuntos.CausaID;
            tbl_Difuntos.LugarID = difuntos.LugarID;
            tbl_Difuntos.NombreApellido = difuntos.NombreApellido.ToUpper();
            tbl_Difuntos.Rut = ValidaRut.Parse(difuntos.Rut.Replace(",", ".").ToUpper());
            tbl_Difuntos.Edad = difuntos.Edad;
            tbl_Difuntos.FechaDefuncion = difuntos.FechaDefuncion.ToUniversalTime();
            tbl_Difuntos.Reuso = difuntos.Reuso ?? null;
            tbl_Difuntos.FechaReuso = difuntos.Reuso == true ? (DateTime?)difuntos.FechaReuso.Value.ToUniversalTime() : null;

            if (difuntos.ID == 0)
            {
                _dbContext.tbl_Difuntos.Add(tbl_Difuntos);
            }
            else
            {
                _dbContext.tbl_Difuntos.AddOrUpdate(tbl_Difuntos);
            }
        }

        private async Task AddServiciosAsync(tbl_Adquisicion adquisicion, int userId, ServicioViewModel model)
        {
            var servicio = model.ID == 0 ? _searchService.GetConceptosServiciosByServicioId(model.ServicioID) : _searchService.GetServicioAdquisicionId(model.ID);
            var valorMoneda = await _searchService.ValorMonedaMesActualAsync(servicio.TipoMonedaID);

            tbl_ServiciosAdquisicion tbl_ServiciosAdquisicion = model.ID == 0 ? new tbl_ServiciosAdquisicion
            {
                AdquisicionID = adquisicion.ID,
                UsuarioID = userId,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            } : adquisicion.tbl_ServiciosAdquisicion.SingleOrDefault(w => w.ID == model.ID);

            tbl_ServiciosAdquisicion.MaestroID = model.MaestroID;

            tbl_ServiciosAdquisicion.Precio = servicio.Precio * valorMoneda.Item1;
            tbl_ServiciosAdquisicion.ServicioID = servicio.ServicioID;
            
            tbl_ServiciosAdquisicion.TipoMonedaID = adquisicion.TipoMonedaID;
            tbl_ServiciosAdquisicion.PrecioTipoMoneda = servicio.Precio;
            tbl_ServiciosAdquisicion.ValorTipoMonedaActual = valorMoneda.Item1;
            tbl_ServiciosAdquisicion.FechaTipoMoneda = valorMoneda.Item2;            

            if (model.ID == 0)
            {
                _dbContext.tbl_ServiciosAdquisicion.Add(tbl_ServiciosAdquisicion);
            }
            else
            {
                _dbContext.tbl_ServiciosAdquisicion.AddOrUpdate(tbl_ServiciosAdquisicion);
            }
        }

        private async Task AddMantencionesAsync(tbl_Adquisicion adquisicion, int userId, MantencionViewModel model)
        {
            var mantencion = model.ID == 0 ? _searchService.GetConceptosMantencionByMantencionId(model.MantencionID) : _searchService.GetMantencionAdquisicionId(model.ID);
            var valorMoneda = await _searchService.ValorMonedaMesActualAsync(mantencion.TipoMonedaID);

            tbl_PagosMantenciones tbl_MantencionAdquisicion = model.ID == 0 ? new tbl_PagosMantenciones
            {
                AdquisicionID = adquisicion.ID,
                UsuarioID = userId,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            } : adquisicion.tbl_PagosMantenciones.SingleOrDefault(w => w.ID == model.ID);

            tbl_MantencionAdquisicion.Precio = model.Precio * valorMoneda.Item1;
            tbl_MantencionAdquisicion.MantencionID = mantencion.MantencionID;
            tbl_MantencionAdquisicion.Anio = model.Anio;
            tbl_MantencionAdquisicion.Observacion = model.Observacion;

            tbl_MantencionAdquisicion.TipoMonedaID = adquisicion.TipoMonedaID;
            tbl_MantencionAdquisicion.PrecioTipoMoneda = mantencion.Precio;
            tbl_MantencionAdquisicion.ValorTipoMonedaActual = valorMoneda.Item1;
            tbl_MantencionAdquisicion.FechaTipoMoneda = valorMoneda.Item2;

            if (model.ID == 0)
            {
                _dbContext.tbl_PagosMantenciones.Add(tbl_MantencionAdquisicion);
            }
            else
            {
                _dbContext.tbl_PagosMantenciones.AddOrUpdate(tbl_MantencionAdquisicion);
            }
        }

        private async Task AddFormaPagoAsync(tbl_Adquisicion adquisicion, int userId, FormaPagoViewModel model)
        {
            tbl_PagoAdquisicion tbl_PagoAdquisicion = model.ID == 0 ? new tbl_PagoAdquisicion
            {
                AdquisicionID = adquisicion.ID,
                UsuarioID = userId,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true,
            } : adquisicion.tbl_PagoAdquisicion.FirstOrDefault(w => w.ID == model.ID);

            tbl_PagoAdquisicion.FormaPagoID = model.FormaPagoID;
            tbl_PagoAdquisicion.NroRecaudacion = model.NroRecaudacion;
            tbl_PagoAdquisicion.Monto = model.Monto;
            tbl_PagoAdquisicion.FechaPago = model.FechaPago.ToUniversalTime();
            tbl_PagoAdquisicion.BancoID = model.BancoID;
            tbl_PagoAdquisicion.NroCheque = model.NroCheque;

            if (model.ID == 0)
            {
                _dbContext.tbl_PagoAdquisicion.Add(tbl_PagoAdquisicion);
            }
            else
            {
                _dbContext.tbl_PagoAdquisicion.AddOrUpdate(tbl_PagoAdquisicion);
            }
        }

        private async Task DiscountStock(int ID)
        {
            var conceptoSector = await _dbContext.tbl_SeccionConceptos.FindAsync(ID);
            conceptoSector.Stock--;
            _dbContext.tbl_SeccionConceptos.AddOrUpdate(conceptoSector);
        }

        private async Task IncreaseStock(int ID)
        {
            var conceptoSector = await _dbContext.tbl_SeccionConceptos.FindAsync(ID);
            conceptoSector.Stock++;
            _dbContext.tbl_SeccionConceptos.AddOrUpdate(conceptoSector);
        }

        private IQueryable<tbl_Adquisicion> GetAdquisicionBaseQueryAsync(Expression<Func<tbl_Adquisicion, bool>> expression = null)
        {
            var query = _dbContext.tbl_Adquisicion
                            .Include(x => x.tbl_Cementerios)
                            .Include(x => x.tbl_Difuntos);

            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query;
        }

        private async Task<List<tbl_Adquisicion>> GetAdquisicionQueryAsync(Expression<Func<tbl_Adquisicion, bool>> expression = null)
        {
            var query = _dbContext.tbl_Adquisicion
                .Include(x => x.tbl_ServiciosAdquisicion)
                .Include(x => x.tbl_PagoAdquisicion)
                .Include(x => x.tbl_Cementerios)
                .Include(x => x.tbl_Conceptos.tbl_SeccionConceptos)
                .Include(x => x.tbl_Secciones)
                .Include(x => x.tbl_Difuntos)
                .Include(x => x.tbl_Estados)
                .Include(x => x.tbl_Observaciones)
                .Include(x => x.tbl_PagosMantenciones)
                .Include(x => x.tbl_HistorialAdquisicion);

            if (expression != null)
            {
                query = query.Where(expression);
            }
            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                item.tbl_Difuntos = item.tbl_Difuntos.Where(d => d.IsActive).ToList();
                item.tbl_ServiciosAdquisicion = item.tbl_ServiciosAdquisicion.Where(d => d.IsActive).ToList();
                item.tbl_PagosMantenciones = item.tbl_PagosMantenciones.Where(d => d.IsActive).ToList();
                item.tbl_PagoAdquisicion = item.tbl_PagoAdquisicion.Where(d => d.IsActive).ToList();
            }
            return result;
        }

        private async Task<tbl_Adquisicion> GetAdquisicionDbByIdAsync(int id)
        {
            var adquisicion = await GetAdquisicionQueryAsync(x => x.ID == id);
            return adquisicion.SingleOrDefault();
        }

        private int CalculateEstado(List<ServicioViewModel> servicios, List<MantencionViewModel> mantencion, List<FormaPagoViewModel> pagos, SeccionConceptosResponse concepto)
        {
            var totalPagos = pagos.Sum(p => p.Monto);
            var resultadoServicios =
                concepto.Precio +
                servicios.Sum(s => s.ID == 0 ? _searchService.GetConceptosServiciosByServicioId(s.ServicioID).Precio : _searchService.GetAdquisicionServiciosResponse(s.ID).Precio) +
                mantencion.Sum(s => s.Precio);

            if (!pagos.Any() && !servicios.Any() && !mantencion.Any() && concepto.Precio == 0)
            {
                return (int)StatusType.Pagado;
            }
            else if (pagos.Any() && servicios.Any() && mantencion.Any() && concepto.Precio == 0)
            {
                return (int)StatusType.Impaga;
            }
            else if (totalPagos == 0 && resultadoServicios > 0)
            {
                return (int)StatusType.Impaga;
            }
            else if (totalPagos < resultadoServicios)
            {
                return (int)StatusType.PagoParcial;
            }
            else if (totalPagos == resultadoServicios)
            {
                return (int)StatusType.Pagado;
            }
            else
            {
                return -1;
            }
        }

        private decimal CalculateSaldoPendiente(List<AdquisicionServiciosResponse> servicios, List<AdquisicionMantencionResponse> mantencion, List<AdquisicionPagoResponse> pagos, decimal valorProducto)
        {
            var totalPagos = pagos.Sum(p => p.Monto);
            var resultadoServicios =
                valorProducto +
                servicios.Sum(s => s.Precio) +
                mantencion.Sum(s => s.Precio);

            return Math.Abs(resultadoServicios - totalPagos);
        }

        private void UpdateComprador(tbl_Adquisicion adquisicion, PersonaViewModel comprador, PersonaViewModel referente)
        {
            adquisicion.CompNombre = comprador.Nombre.ToUpper();
            adquisicion.CompApellido = comprador.Apellido.ToUpper();
            adquisicion.CompNum = comprador.DirNum.ToUpper();
            adquisicion.CompEmail = comprador.Email != null ? comprador.Email.ToUpper() : null;
            adquisicion.CompDireccion = comprador.Direccion1.ToUpper();
            adquisicion.CompTelefono = comprador.Telefono.ToUpper();
            adquisicion.CompRut = comprador.Rut.ToUpper();
            adquisicion.CompComunaID = comprador.ComunaID;

            adquisicion.RefNombre = referente.Nombre.ToUpper();
            adquisicion.RefApellido = referente.Apellido.ToUpper();
            adquisicion.RefNum = referente.DirNum.ToUpper();
            adquisicion.RefEmail = referente.Email != null ? referente.Email.ToUpper() : null;
            adquisicion.RefDireccion = referente.Direccion1.ToUpper();
            adquisicion.RefTelefono = referente.Telefono.ToUpper();
            adquisicion.RefRut = referente.Rut.ToUpper();
            adquisicion.RefComunaID = referente.ComunaID;
        }

        private async Task AddObservacion(int id, string observacion, int userId)
        {
            if (!string.IsNullOrEmpty(observacion))
            {
                tbl_Observaciones tbl_Observaciones = new tbl_Observaciones
                {
                    UsuarioID = userId,
                    FechaRegistro = DateTime.UtcNow,
                    AdquisicionID = id,
                    Observacion = observacion.ToUpper(),
                };
                _dbContext.tbl_Observaciones.Add(tbl_Observaciones);
            }
        }

        private async Task AddHistorial(int id, string accion, int userId)
        {
            if (!string.IsNullOrEmpty(accion))
            {
                tbl_HistorialAdquisicion historial = new tbl_HistorialAdquisicion
                {
                    UsuarioID = userId,
                    FechaRegistro = DateTime.UtcNow,
                    AdquisicionID = id,
                    Accion = accion.ToUpper(),
                };
                _dbContext.tbl_HistorialAdquisicion.Add(historial);
            }
        }

        private async Task<string> GetNextNroFichaAsync(int cementerioId, bool isRegulated)
        {
            var fichas = await _dbContext.tbl_Adquisicion
                .Where(x => x.CementerioID == cementerioId && !string.IsNullOrEmpty(x.NroFicha))
                .Select(x => x.NroFicha)
                .ToListAsync();

            var maxNroFicha = fichas
                .OrderByDescending(nroFicha => int.Parse(System.Text.RegularExpressions.Regex.Match(nroFicha, @"\d+").Value))
                .FirstOrDefault();

            int nextFichaNumber = 100; 
            if (!string.IsNullOrEmpty(maxNroFicha))
            {
                var numericPart = new string(maxNroFicha.SkipWhile(c => !char.IsDigit(c)).ToArray());
                if (int.TryParse(numericPart, out var maxNumber))
                {
                    nextFichaNumber = maxNumber + 1;
                }
            }

            return isRegulated ? $"R{nextFichaNumber}" : $"N{nextFichaNumber}";
        }
    }
}