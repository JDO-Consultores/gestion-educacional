using Gestionador.Models;
using Gestionador.Responses;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IConfigService
    {
        Task<PersonaResponse> GetMaestroById(int id);
        Task<CausaReponse> GetCausaById(int id);
        Task<LugarDefuncionResponse> GetLugerDefuncionById(int id);
        Task<ParrocosResponse> GetParrocoById(int id);
        Task<ValorMonedasResponse> GetValorMonedaById(int id);
        Task<int> UpsertCausaAsync(CausaRequest request);
        Task<int> UpsertMaestroAsync(PersonaViewModel request);
        Task<int> UpsertTipoMonedaAsync(ValorMonedaRequest request);
        Task<int> UpsertLugarDefuncionAsync(LugarDefuncionRequest request);        
        Task<int> UpsertParrocoAsync(ParrocoRequest request);
    }
}