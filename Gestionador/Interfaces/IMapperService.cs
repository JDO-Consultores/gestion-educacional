using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;

namespace Gestionador.Interfaces
{
    public interface IMapperService
    {
        UserResponse MapToUserResponse(tbl_Usuarios entity);
        RolesResponse MapToRolResponse(tbl_Roles entity);
        RolesUsuarios MapToRolesUsuarios(tbl_RolesUsuarios entity);
        RegionesResponse MapToRegionesResponse(tbl_Regiones entity);
        ComunasResponse MapToComunasResponse(tbl_Comunas entity);
        CementerioReponse MapToCementeriosResponse(tbl_Cementerios entity);
        ConceptoResponse MapToConceptosResponse(tbl_Conceptos entity);
        FormasPagoResponse MapToFormasPagoResponse(tbl_FormasPago entity);
        CausaReponse MapToCausasResponse(tbl_Causas entity);
        TipoMonedaResponse MapToTipoMonedaResponse(tbl_TipoMonedas entity);
        ValorMonedasResponse MapToValorMonedasResponse(tbl_ValorMonedas entity);
        BancosResponse MapToBancoResponse(tbl_Bancos entity);
        LugarDefuncionResponse MapToLugarDefuncionResponse(tbl_LugarDefuncion entity);
        PatiosResponse MapToPatiosResponse(tbl_Patios entity);
        SectoresResponse MapToSectoresResponse(tbl_Secciones entity);
        SectoresResponse MapToSectoresPatioResponse(tbl_Secciones entity);
        SeccionConceptosResponse MapToSeccionConceptoResponse(tbl_SeccionConceptos entity);
        ServiciosResponse MapToServicioResponse(tbl_Servicios entity);
        ServiciosConceptosResponse MapToServicioConceptoResponse(tbl_ConceptosServicios entity);
        MantencionConceptosResponse MapToMantencionConceptoResponse(tbl_ConceptosServicios entity);
        ServiciosConceptosResponse MapToServicioConceptoResponse(tbl_ServiciosAdquisicion entity);
        MantencionConceptosResponse MapToMantencionConceptoResponse(tbl_PagosMantenciones entity);
        PersonaResponse MapToCompradorResponse(dynamic entity, bool isComprador);
        PersonaResponse MapToMaestroResponse(dynamic entity);
        DifuntoViewModel MapToDifuntosResponse(tbl_Difuntos entity);
        AdquisicionResponse MapToAdquisicionResponse(tbl_Adquisicion entity);
        AdquisicionBase MapToAdquisicionBaseResponse(tbl_Adquisicion entity);
        AdquisicionObservacion MapToAdquisicionObservacion(tbl_Observaciones entity);
        HistorialResponse MapToHistorialResponse(tbl_HistorialAdquisicion entity);
        AdquisicionServiciosResponse MapToAdquisicionServiciosResponse(tbl_ServiciosAdquisicion entity);
        AdquisicionMantencionResponse MapToAdquisicionMantencionResponse(tbl_PagosMantenciones entity);
        ParrocosResponse MapToParrocosResponse(tbl_Parrocos entity);
        CategoriasResponse MapToCategoriaResponse(tbl_Categorias entity);
        TipoAdministradorResponse MapToTipoAdministradorResponse(tbl_TipoAdministrador entity);
    }
}