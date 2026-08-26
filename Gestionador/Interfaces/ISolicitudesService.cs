using Gestionador.Models;
using Gestionador.Responses;
using KendoNET.DynamicLinq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface ISolicitudesService
    {
        Task<int> CreateAdquisicionAsync(int userId, FichaViewModelBase model);
        Task<DataSourceResult> GetSolicitudesIndexAsync(DataSourceRequest request);
        Task<AdquisicionResponse> GetSolicitudById(int id);
        Task<int> EditAdquisicionAsync(int userId, int ID, FichaViewModel model);
        Task<int> TransferAdquisicionAsync(int userId, TransferAdquisicionViewModel model);
        Task<byte[]> GenerateReport(int id, int parrocoId, int userId, List<CheckState> selectedChecks);
        Task<List<AdquisicionBase>> BuscarAdquisicionAsync(string dropdownValue, string searchText);
        Task<bool> AnularFichaAsync(int userId, int id);
    }
}