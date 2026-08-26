using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IProfesorJefeService
    {
        Task<DataSourceResult> GetProfesoresAsync(DataSourceRequest request);
        Task<ProfesorJefeViewModel> GetByIdAsync(int id);
        Task<int> CreateAsync(ProfesorJefeViewModel model, string usuario);
        Task<int> UpdateAsync(ProfesorJefeViewModel model, string usuario);
        Task<bool> RutExisteAsync(string rut, int excludeId = 0);
        Task<List<SelectItemViewModel>> GetEstadosProfesorAsync();
    }
}
