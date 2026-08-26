using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class PatiosServices : BaseServices, IPatiosService
    {
        public PatiosServices(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<PatiosResponse> GetPatioAsync(int id)
        {
            var result = await _dbContext.tbl_Patios.SingleOrDefaultAsync(x => x.ID == id);
            return _mapperService.MapToPatiosResponse(result);
        }

        public async Task<SeccionConceptosResponse> GetSeccionConceptoByIdAsync(int id)
        {
            var seccionConceptos = _dbContext.tbl_SeccionConceptos
                                        .Include(sc => sc.tbl_Conceptos).Include(sc => sc.tbl_Secciones).Include(sc => sc.tbl_Secciones.tbl_Patios)
                                        .Where(sc => sc.ID == id)
                                        .ToList();

            var seccionId = seccionConceptos.FirstOrDefault().SeccionID;
            var conceptoId = seccionConceptos.FirstOrDefault().ConceptoID;

            var response = seccionConceptos
                            .Select(sc =>
                            {
                                var seccionConceptoResponse = new SeccionConceptosResponse
                                {
                                    ID = sc.ID,
                                    SeccionID = sc.SeccionID,
                                    ConceptoID = sc.ConceptoID,
                                    Stock = sc.Stock,
                                    Precio = sc.Precio,
                                    IsActive = sc.IsActive,
                                    TipoMonedaID = sc.TipoMonedaID,
                                    Concepto = _mapperService.MapToConceptosResponse(sc.tbl_Conceptos),
                                    Sectores = _mapperService.MapToSectoresPatioResponse(sc.tbl_Secciones)
                                };

                                seccionConceptoResponse.Concepto.ServiciosConceptos = _dbContext.tbl_ConceptosServicios
                                    .Where(w => w.ConceptoID == conceptoId && w.SeccionID == seccionId && w.IsActive)
                                    .ToList()
                                    .Select(s => _mapperService.MapToServicioConceptoResponse(s))
                                    .ToList();

                                return seccionConceptoResponse;
                            })
                            .OrderBy(r => r.Sectores.Patios)
                            .ThenBy(r => r.Sectores.Sector)
                            .SingleOrDefault();

            return response;
        }

        public async Task<List<dynamic>> GetPatiosByCementerioId(int id)
        {
            var patios = _dbContext.tbl_Patios.Where(w => w.CementerioID == id).OrderBy(w => w.Patio).ToList();
            var patiosResponse = patios.Select(s => _mapperService.MapToPatiosResponse(s)).ToList<dynamic>();
            return patiosResponse;
        }

        public async Task<List<PatiosResponse>> GetPatiosAsync()
        {
            var patios = _dbContext.tbl_Patios.Include(p => p.tbl_Secciones
                  .Select(s => s.tbl_SeccionConceptos.Select(sc => sc.tbl_Conceptos)))
                  .Select(_mapperService.MapToPatiosResponse).OrderBy(x => x.Patio).ThenBy(w => w.Sectores).ToList();

            foreach (var patio in patios)
            {
                foreach (var sector in patio.Sectores)
                {
                    foreach (var seccionConcepto in sector.SeccionConcepto)
                    {
                        seccionConcepto.Concepto.ServiciosConceptos.Clear();
                        seccionConcepto.Concepto.ServiciosConceptos.AddRange(
                                    _dbContext.tbl_ConceptosServicios.Where(w => w.ConceptoID == seccionConcepto.ConceptoID && w.SeccionID == sector.ID && w.IsActive).ToList()
                                    .Select(s => _mapperService.MapToServicioConceptoResponse(s)).ToList());
                    }
                }
            }

            return patios;
        }

        public async Task<List<CementerioReponse>> GetCementeriosAsync()
        {
            var cementerios = _dbContext.tbl_Cementerios.Include(x => x.tbl_Patios.Select(
                                        s => s.tbl_Secciones.Select(a => a.tbl_SeccionConceptos))).Select(_mapperService.MapToCementeriosResponse)
                                        .OrderBy(o => o.Cementerio).ToList();

            foreach (var cementerio in cementerios)
            {
                foreach (var patio in cementerio.Patios)
                {
                    foreach (var sector in patio.Sectores)
                    {
                        foreach (var seccionConcepto in sector.SeccionConcepto)
                        {
                            seccionConcepto.Concepto.ServiciosConceptos.Clear();
                            seccionConcepto.Concepto.ServiciosConceptos.AddRange(
                                        _dbContext.tbl_ConceptosServicios.Where(w => w.ConceptoID == seccionConcepto.ConceptoID && w.SeccionID == sector.ID && w.IsActive).ToList()
                                        .Select(s => _mapperService.MapToServicioConceptoResponse(s)).ToList());
                        }
                    }
                }            
            }

            return cementerios;
        }

        public async Task<int> CreatePatiosAsync(PatioRequest request)
        {
            var exist = await _dbContext.tbl_Patios.AnyAsync(w => w.Patio.Equals(request.Patio) && w.CementerioID == request.CementerioID);
            if (exist)
            {
                return -1;
            }

            var patio = new tbl_Patios
            {
                Patio = request.Patio,
                CementerioID = request.CementerioID,
                IsActive = true
            };
            _dbContext.tbl_Patios.Add(patio);

            if (request.Sectores != null)
            {
                foreach (var item in request.Sectores)
                {
                    _dbContext.tbl_Secciones.Add(new tbl_Secciones
                    {
                        Seccion = item.Sector.ToUpper(),
                        PatioID = patio.ID,
                        IsActive = true
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
            return patio.ID;
        }

        public async Task<int> EditPatiosAsync(PatioRequest request)
        {
            var patio = await _dbContext.tbl_Patios.FindAsync(request.ID);

            patio.IsActive = (bool)request.IsActive;

            if (request.Sectores?.Any() ?? false)
            {
                patio.tbl_Secciones.Where(w => w.ID != 0 && !request.Sectores.Any(a => w.ID == a.ID)).ForEach(f => { f.IsActive = false; });
                foreach (var item in request.Sectores)
                {
                    if (item.ID == 0)
                    {
                        _dbContext.tbl_Secciones.Add(new tbl_Secciones
                        {
                            Seccion = item.Sector.ToUpper(),
                            PatioID = patio.ID,                            
                            IsActive = true
                        });
                    }                    
                }
            }

            await _dbContext.SaveChangesAsync();
            return patio.ID;
        }

        public async Task<int> CreateSeccionConcepto(SeccionConceptoRequest request)
        {
            var exist = await _dbContext.tbl_SeccionConceptos.AnyAsync(w => w.ConceptoID == request.ConceptoID && w.SeccionID == request.SeccionID && w.IsActive);
            if (exist)
            {
                return -1;
            }
            var seccionConcepto = new tbl_SeccionConceptos
            {
                ConceptoID = request.ConceptoID,
                IsActive = request.IsActive,
                Precio = request.Precio,
                Stock = request.Stock,
                SeccionID = request.SeccionID,
                TipoMonedaID = request.TipoMonedaID
            };

            _dbContext.tbl_SeccionConceptos.Add(seccionConcepto);
            foreach (var item in request.ServicioRequests)
            {
                item.ConceptoID = seccionConcepto.ConceptoID;
                item.SeccionID = seccionConcepto.SeccionID;
                var conceptosServicios = new tbl_ConceptosServicios
                {
                    SeccionID = item.SeccionID,
                    Precio = item.Precio,
                    ConceptoID = item.ConceptoID,
                    ServicioID = item.ServicioID,
                    IsActive = true,
                    TipoMonedaID = request.TipoMonedaID
                };
                _dbContext.tbl_ConceptosServicios.Add(conceptosServicios);
            }

            await _dbContext.SaveChangesAsync();
            return seccionConcepto.ID;
        }

        public async Task<int> EditSeccionConceptoAsync(SeccionConceptoRequest request)
        {
            var seccionConcepto = _dbContext.tbl_SeccionConceptos.Find(request.ID);
            seccionConcepto.Precio = request.Precio;
            seccionConcepto.Stock = request.Stock;
            seccionConcepto.IsActive = request.IsActive;
            seccionConcepto.TipoMonedaID = request.TipoMonedaID;

            if (request.ServicioRequests?.Any() ?? false)
            {
                var serviciosConcepto = _dbContext.tbl_ConceptosServicios.Where(w => w.ConceptoID == request.ConceptoID && w.SeccionID == request.SeccionID).ToList();
                serviciosConcepto.Where(w => w.ID != 0 && !request.ServicioRequests.Any(a => a.ID == w.ID)).ForEach(f => f.IsActive = false);
                foreach (var item in request.ServicioRequests)
                {
                    tbl_ConceptosServicios conceptosServicios = item.ID == null ? new tbl_ConceptosServicios
                    {
                        SeccionID = item.SeccionID,
                        ConceptoID = item.ConceptoID,
                        ServicioID = item.ServicioID

                    } : _dbContext.tbl_ConceptosServicios.Find(item.ID);

                    conceptosServicios.IsActive = true;
                    conceptosServicios.Precio = item.Precio;
                    conceptosServicios.TipoMonedaID = item.TipoMonedaID;

                    if (item.ID.HasValue)
                    {
                        _dbContext.tbl_ConceptosServicios.AddOrUpdate(conceptosServicios);
                    }
                    else
                    {
                        _dbContext.tbl_ConceptosServicios.Add(conceptosServicios);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
            return seccionConcepto.ID;
        }
    }
}