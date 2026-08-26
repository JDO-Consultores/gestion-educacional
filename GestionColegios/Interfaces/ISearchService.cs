using GestionColegios.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface ISearchService
    {
        List<dynamic> GetRegiones();
        Task<List<ComunasResponse>> GetComunasByRegionIdAsync(int id);
    }
}