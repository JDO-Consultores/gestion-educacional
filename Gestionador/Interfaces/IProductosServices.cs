using Gestionador.Models;
using Gestionador.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IProductosServices
    {
        Task<List<ConceptoResponse>> GetProductosAsync();
        Task<ConceptoResponse> GetProductoById(int id);
        Task<int> UpsertProductoAsync(ConceptoRequest request);

    }
}