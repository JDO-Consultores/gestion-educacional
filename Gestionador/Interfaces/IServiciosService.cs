using Gestionador.Models;
using Gestionador.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IServiciosService
    {
        Task<List<ServiciosResponse>> GetServicios();
        Task<ServiciosResponse> GetServicioById(int id);
        Task<int> UpsertServicioAsync(ServicioRequest request);
    }
}
