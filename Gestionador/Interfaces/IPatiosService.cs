using Gestionador.Models;
using Gestionador.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IPatiosService
    {
        Task<SeccionConceptosResponse> GetSeccionConceptoByIdAsync(int id);
        Task<List<PatiosResponse>> GetPatiosAsync();
        Task<List<dynamic>> GetPatiosByCementerioId(int id);
        Task<int> CreateSeccionConcepto(SeccionConceptoRequest request);
        Task<int> EditSeccionConceptoAsync(SeccionConceptoRequest request);
        Task<int> CreatePatiosAsync(PatioRequest request);
        Task<PatiosResponse> GetPatioAsync(int id);
        Task<int> EditPatiosAsync(PatioRequest request);
        Task<List<CementerioReponse>> GetCementeriosAsync();
    }
}
