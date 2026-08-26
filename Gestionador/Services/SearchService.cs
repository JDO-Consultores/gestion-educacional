using Gestionador.Helpers.Enum;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Responses;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Services
{
    [Authorize]
    public class SearchService : BaseServices, ISearchService
    {
        public SearchService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public List<dynamic> GetRegiones()
        {
            var regiones = _dbContext.tbl_Regiones.OrderBy(w => w.NroRegion).ToList();
            var regionesResponse = regiones.Select(s => _mapperService.MapToRegionesResponse(s)).ToList<dynamic>();
            return regionesResponse;
        }

        public async Task<List<ComunasResponse>> GetComunasByRegionIdAsync(int id)
        {
            var comunas = await _dbContext.tbl_Comunas.Where(w => w.RegionID == id && w.IsActive == true).ToListAsync();
            var comunasResponse = comunas.Select(s => _mapperService.MapToComunasResponse(s)).OrderBy(o => o.Comuna).ToList();
            return comunasResponse;
        }

        public List<dynamic> GetTipoMonedas()
        {
            var tipoMonedas = _dbContext.tbl_TipoMonedas.Where(w => w.IsActive == true).ToList();
            var tipoMonedasResponse = tipoMonedas.Select(s => _mapperService.MapToTipoMonedaResponse(s)).ToList<dynamic>();
            return tipoMonedasResponse;
        }

        public List<dynamic> GetCementerios()
        {
            var cementerios = _dbContext.tbl_Cementerios.Where(w => w.IsActive == true).ToList();
            var cementeriosResponse = cementerios.Select(s => _mapperService.MapToCementeriosResponse(s)).ToList<dynamic>();
            return cementeriosResponse;
        }

        public SeccionConceptosResponse GetSeccionConceptoById(int id)
        {
            var conceptos = _dbContext.tbl_SeccionConceptos.Find(id);
            var conceptosResponse = _mapperService.MapToSeccionConceptoResponse(conceptos);
            return conceptosResponse;
        }

        public SeccionConceptosResponse GetSeccionConceptoBySeccionConceptoId(int sectorId, int conceptoId)
        {
            var conceptos = _dbContext.tbl_SeccionConceptos.Where(w => w.SeccionID == sectorId && w.ConceptoID == conceptoId).SingleOrDefault();
            var conceptosResponse = _mapperService.MapToSeccionConceptoResponse(conceptos);
            return conceptosResponse;
        }

        public ConceptoResponse GetConceptoById(int id)
        {
            return _mapperService.MapToConceptosResponse(_dbContext.tbl_Conceptos.Find(id));
        }

        public async Task<TipoMonedaResponse> GetTipoMonedaByID(int id)
        {
            return _mapperService.MapToTipoMonedaResponse(await _dbContext.tbl_TipoMonedas.FindAsync(id));
        }

        public List<dynamic> GetServiciosById(int[] ids, bool onlyOne = false)
        {
            List<tbl_Servicios> servicios = new List<tbl_Servicios>();
            if (onlyOne)
            {
                servicios = _dbContext.tbl_Servicios.Where(w => w.CategoriaID != 1 && w.IsActive == true).OrderBy(o => o.Servicio).ToList();
            }
            else
            {
                servicios = _dbContext.tbl_Servicios.Where(w => ids.Contains(w.CategoriaID) && w.IsActive == true).OrderBy(o => o.Servicio).ToList();
            }
            var servicioResponse = servicios.Select(s => _mapperService.MapToServicioResponse(s)).ToList<dynamic>();
            return servicioResponse;
        }

        public List<dynamic> GetServiciosByConceptoId(int id, int seccionId)
        {
            var concepto = GetSeccionConceptoById(id);
            var serviciosConceptos = _dbContext.tbl_ConceptosServicios.Include(x => x.tbl_Servicios)
                                                .Where(w => w.ConceptoID == concepto.ConceptoID && w.SeccionID == seccionId && w.tbl_Servicios.IsActive).ToList();
            var scResponse = serviciosConceptos.Select(s => _mapperService.MapToServicioConceptoResponse(s)).ToList<dynamic>();
            return scResponse;
        }

        public List<dynamic> GetServiciosCategories(int id, int seccionId)
        {
            var concepto = GetSeccionConceptoById(id);
            var serviciosConceptos = _dbContext.tbl_ConceptosServicios.Include(x => x.tbl_Servicios).Where(w => w.ConceptoID == concepto.ConceptoID && w.SeccionID == seccionId
                                    && w.tbl_Servicios.CategoriaID != 1 && w.tbl_Servicios.CategoriaID != 4).ToList();
            var scResponse = serviciosConceptos.Select(s => _mapperService.MapToServicioConceptoResponse(s)).ToList<dynamic>();
            return scResponse;
        }

        public List<dynamic> GetMantencionesCategories(int id, int seccionId)
        {
            var concepto = GetSeccionConceptoById(id);
            var serviciosConceptos = _dbContext.tbl_ConceptosServicios.Include(x => x.tbl_Servicios).Where(w => w.ConceptoID == concepto.ConceptoID && w.SeccionID == seccionId
                                    && w.tbl_Servicios.CategoriaID == 4).ToList();
            var scResponse = serviciosConceptos.Select(s => _mapperService.MapToMantencionConceptoResponse(s)).ToList<dynamic>();
            return scResponse;
        }

        public List<dynamic> GetConceptosBySectorId(int id)
        {
            var seccionConcepto = _dbContext.tbl_SeccionConceptos.Include(x => x.tbl_Conceptos).Where(w => w.SeccionID == id && w.IsActive && w.tbl_Conceptos.IsActive).ToList();
            var conceptoResponse = seccionConcepto.Select(s => _mapperService.MapToSeccionConceptoResponse(s)).ToList<dynamic>();
            return conceptoResponse;
        }

        public List<dynamic> GetFormasPago()
        {
            var formasPago = _dbContext.tbl_FormasPago.Where(w => w.IsActive == true).ToList();
            var formasPagoResponse = formasPago.Select(s => _mapperService.MapToFormasPagoResponse(s)).ToList<dynamic>();
            return formasPagoResponse;
        }

        public FormasPagoResponse GetFormaPagoById(int id)
        {
            return _mapperService.MapToFormasPagoResponse(_dbContext.tbl_FormasPago.Find(id));
        }
        
        public List<dynamic> GetValorMonedasAdmin()
        {
            var valorMoneda = _dbContext.tbl_ValorMonedas.ToList();
            var valorMonedaResponse = valorMoneda.Select(s => _mapperService.MapToValorMonedasResponse(s)).ToList<dynamic>();
            return valorMonedaResponse;
        }

        public List<dynamic> GetCausas()
        {
            var causas = _dbContext.tbl_Causas.Where(w => w.IsActive == true).ToList();
            var causasResponse = causas.Select(s => _mapperService.MapToCausasResponse(s)).ToList<dynamic>();
            return causasResponse;
        }

        public List<dynamic> GetCausasAdmin()
        {
            var causas = _dbContext.tbl_Causas.ToList();
            var causasResponse = causas.Select(s => _mapperService.MapToCausasResponse(s)).ToList<dynamic>();
            return causasResponse;
        }

        public List<dynamic> GetMaestros()
        {
            var causas = _dbContext.tbl_Maestros.Where(w => w.IsActive == true).ToList();
            var causasResponse = causas.Select(s => _mapperService.MapToMaestroResponse(s)).ToList<dynamic>();
            return causasResponse;
        }

        public List<dynamic> GetMaestrosAdmin()
        {
            var causas = _dbContext.tbl_Maestros.ToList();
            var causasResponse = causas.Select(s => _mapperService.MapToMaestroResponse(s)).ToList<dynamic>();
            return causasResponse;
        }

        public List<dynamic> GetBancos()
        {
            var banco = _dbContext.tbl_Bancos.Where(w => w.IsActive == true).OrderBy(x => x.Banco).ToList();
            var bancoResponse = banco.Select(s => _mapperService.MapToBancoResponse(s)).ToList<dynamic>();
            return bancoResponse;
        }

        public List<dynamic> GetLugarDefuncionAdmin()
        {
            var lugar = _dbContext.tbl_LugarDefuncion.ToList();
            var lugarResponse = lugar.Select(s => _mapperService.MapToLugarDefuncionResponse(s)).ToList<dynamic>();
            return lugarResponse;
        }

        public List<dynamic> GetLugarDefuncion()
        {
            var lugar = _dbContext.tbl_LugarDefuncion.Where(w => w.IsActive == true).ToList();
            var lugarResponse = lugar.Select(s => _mapperService.MapToLugarDefuncionResponse(s)).ToList<dynamic>();
            return lugarResponse;
        }

        public List<dynamic> GetCategorias(int[] ids)
        {
            var categorias = _dbContext.tbl_Categorias.Where(w => w.IsActive == true && w.ID != 1).ToList();
            var categoriasResponse = categorias.Select(s => _mapperService.MapToCategoriaResponse(s)).ToList<dynamic>();
            return categoriasResponse;
        }

        public List<dynamic> GetTipoAdministrador()
        {
            var tipoAdministrador = _dbContext.tbl_TipoAdministrador.ToList();
            var tipoAdminResponse = tipoAdministrador.Select(s => _mapperService.MapToTipoAdministradorResponse(s)).ToList<dynamic>();
            return tipoAdminResponse;
        }

        public List<dynamic> GetPatios()
        {
            var patios = _dbContext.tbl_Patios.Where(w => w.IsActive).OrderBy(x => x.Patio).ToList();
            var patiosResponse = patios.Select(s => _mapperService.MapToPatiosResponse(s)).ToList<dynamic>();
            return patiosResponse;
        }

        public List<dynamic> GetPatiosByCementerioID(int cementerioId)
        {
            var patios = _dbContext.tbl_Patios.Where(w => w.IsActive && w.CementerioID == cementerioId).OrderBy(x => x.Patio).ToList();
            var patiosResponse = patios.Select(s => _mapperService.MapToPatiosResponse(s)).ToList<dynamic>();
            return patiosResponse;
        }

        public List<dynamic> GetLetrasNichos()
        {
            var letras = _dbContext.tbl_Letra.OrderBy(x => x.Letra).ToList();
            var letrasResponse = letras.ToList<dynamic>();
            return letrasResponse;
        }

        public async Task<List<SectoresResponse>> GetSectoresByIdAsync(int id)
        {
            var sectores = await _dbContext.tbl_Secciones.Where(w => w.PatioID == id && w.IsActive == true).ToListAsync();
            var sectoresResposne = sectores.Select(s => _mapperService.MapToSectoresResponse(s)).OrderBy(o => o.Sector).ToList();
            return sectoresResposne;
        }

        public ServiciosConceptosResponse GetConceptosServiciosByServicioId(int id)
        {
            return _mapperService.MapToServicioConceptoResponse(_dbContext.tbl_ConceptosServicios.Find(id));
        }

        public ServiciosConceptosResponse GetServicioAdquisicionId(int id)
        {
            return _mapperService.MapToServicioConceptoResponse(_dbContext.tbl_ServiciosAdquisicion.Find(id));
        }

        public MantencionConceptosResponse GetConceptosMantencionByMantencionId(int id)
        {
            return _mapperService.MapToMantencionConceptoResponse(_dbContext.tbl_ConceptosServicios.Find(id));
        }

        public MantencionConceptosResponse GetMantencionAdquisicionId(int id)
        {
            return _mapperService.MapToMantencionConceptoResponse(_dbContext.tbl_PagosMantenciones.Find(id));
        }

        public ServiciosConceptosResponse GetConceptosServiciosByServicioConceptoId(int servicioId, int conceptoId)
        {
            return _mapperService.MapToServicioConceptoResponse(_dbContext.tbl_ConceptosServicios.Where(w => w.ServicioID == servicioId && w.ConceptoID == conceptoId && w.IsActive == true).SingleOrDefault());
        }

        public ServiciosConceptosResponse GetConceptosServiciosBySeccionConceptoId(int seccionID, int conceptoId)
        {
            return _mapperService.MapToServicioConceptoResponse(_dbContext.tbl_ConceptosServicios.Where(w => w.SeccionID == seccionID && w.ConceptoID == conceptoId && w.IsActive == true).SingleOrDefault());
        }

        public AdquisicionServiciosResponse GetAdquisicionServiciosResponse(int id)
        {
            return _mapperService.MapToAdquisicionServiciosResponse(_dbContext.tbl_ServiciosAdquisicion.Find(id));
        }

        public AdquisicionMantencionResponse GetAdquisicionMantencionResponse(int id)
        {
            return _mapperService.MapToAdquisicionMantencionResponse(_dbContext.tbl_PagosMantenciones.Find(id));
        }

        public PersonaResponse GetPersonaByRut(string rut)
        {
            var persona = _dbContext.tbl_Adquisicion
                                .Include(x => x.tbl_Comunas)
                                .Include(x => x.tbl_Comunas.tbl_Regiones).OrderByDescending(x => x.ID)
                                .FirstOrDefault(w => w.CompRut.Equals(rut) || w.RefRut.Equals(rut));
            return _mapperService.MapToCompradorResponse(persona, true);
        }

        public List<dynamic> GetAnios()
        {
            List<object> list = new List<object>();
            int currentYear = DateTime.Now.Year;
            list.Add(new { ID = currentYear + 1, Val = currentYear + 1 });
            for (int i = 0; i <= 20; i++)
            {
                list.Add(new { ID = currentYear - i, Val = currentYear - i });
            }
            return list.ToList<dynamic>();
        }

        public List<dynamic> GetParrocos()
        {
            var parrocos = _dbContext.tbl_Parrocos.Where(w => w.IsActive).ToList();
            var parrocosResponse = parrocos.Select(s => _mapperService.MapToParrocosResponse(s)).ToList<dynamic>();
            return parrocosResponse;
        }

        public async Task<ParrocosResponse> GetParrocoById(int id)
        {
            return _mapperService.MapToParrocosResponse(await _dbContext.tbl_Parrocos.FindAsync(id));
        }

        public List<dynamic> GetConceptosAsync()
        {
            var concepto = _dbContext.tbl_Conceptos.Where(w => w.IsActive).OrderBy(x => x.Concepto).ToList();
            var conceptoResponse = concepto.Select(s => _mapperService.MapToConceptosResponse(s)).ToList<dynamic>();
            return conceptoResponse;
        }

        public async Task<(decimal, DateTime)> ValorMonedaMesActualAsync(int tipoMonedaId)
        {
            var tipoMoneda = await GetTipoMonedaByID(tipoMonedaId);
            var mes = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var day = DateTime.Now.Day;
            decimal valor = 1;
            DateTime dateTime;
            if (tipoMoneda.ID == 1)
            {
                return (1, DateTime.Now);
            }
            if (tipoMoneda.Periodicidad == PeriodicityType.DIARIA.ToString())
            {
                var result = await _dbContext.tbl_ValorMonedas.Where(u => u.Mes == mes && u.Year == year && u.Dia == day && u.TipoMonedaID == tipoMoneda.ID).OrderByDescending(u => u.FechaRegistro).FirstOrDefaultAsync();
                valor = result.Valor;
                dateTime = new DateTime(result.Year, result.Mes, result.Dia);
            }
            else if (tipoMoneda.Periodicidad == PeriodicityType.MENSUAL.ToString())
            {
                var result = await _dbContext.tbl_ValorMonedas.Where(u => u.Mes == mes && u.Year == year && u.TipoMonedaID == tipoMoneda.ID).OrderByDescending(u => u.FechaRegistro).FirstOrDefaultAsync();
                valor = result.Valor;
                dateTime = new DateTime(result.Year, result.Mes, result.Dia);
            }
            else
            {
                return (-1, DateTime.Now);
            }
            return (valor, dateTime);
        }
    }
}