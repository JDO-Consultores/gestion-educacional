using Gestionador.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface ISearchService
    {
        List<dynamic> GetRegiones();
        Task<List<ComunasResponse>> GetComunasByRegionIdAsync(int id);
        List<dynamic> GetTipoMonedas();
        List<dynamic> GetCementerios();
        SeccionConceptosResponse GetSeccionConceptoById(int id);
        SeccionConceptosResponse GetSeccionConceptoBySeccionConceptoId(int sectorId, int conceptoId);
        List<dynamic> GetServiciosById(int[] ids, bool onlyOne = false);
        List<dynamic> GetConceptosBySectorId(int id);
        ConceptoResponse GetConceptoById(int id);
        Task<TipoMonedaResponse> GetTipoMonedaByID(int id);
        List<dynamic> GetFormasPago();
        FormasPagoResponse GetFormaPagoById(int id);
        List<dynamic> GetCausas();
        List<dynamic> GetCausasAdmin();
        List<dynamic> GetMaestros();
        List<dynamic> GetMaestrosAdmin();
        List<dynamic> GetValorMonedasAdmin();
        List<dynamic> GetBancos();
        List<dynamic> GetLugarDefuncion();
        List<dynamic> GetLugarDefuncionAdmin();
        List<dynamic> GetPatios();
        List<dynamic> GetPatiosByCementerioID(int cementerioId);
        List<dynamic> GetLetrasNichos();
        Task<List<SectoresResponse>> GetSectoresByIdAsync(int id);
        List<dynamic> GetServiciosByConceptoId(int id, int seccionId);
        ServiciosConceptosResponse GetConceptosServiciosByServicioId(int id);
        MantencionConceptosResponse GetConceptosMantencionByMantencionId(int id);
        PersonaResponse GetPersonaByRut(string rut);
        List<dynamic> GetServiciosCategories(int id, int seccionId);
        List<dynamic> GetMantencionesCategories(int id, int seccionId);
        ServiciosConceptosResponse GetConceptosServiciosBySeccionConceptoId(int seccionID, int conceptoId);
        ServiciosConceptosResponse GetConceptosServiciosByServicioConceptoId(int servicioId, int conceptoId);
        List<dynamic> GetAnios();
        List<dynamic> GetCategorias(int[] ids);
        List<dynamic> GetTipoAdministrador();
        List<dynamic> GetParrocos();
        Task<ParrocosResponse> GetParrocoById(int id);
        List<dynamic> GetConceptosAsync();
        Task<(decimal, DateTime)> ValorMonedaMesActualAsync(int tipoMonedaId);
    }
}