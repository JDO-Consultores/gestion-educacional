using GestionColegios.ViewModels;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IApoderadoService
    {
        Task<ApoderadoFormViewModel> GetFormDataAsync(int alumnoId, int? alumnoApoderadoId = null);
        Task<int> GuardarApoderadoAsync(ApoderadoFormViewModel model, string createdBy);
        Task<int> DesvincularApoderadoAsync(int alumnoApoderadoId, string modifiedBy);
        Task<ApoderadoViewModel> BuscarPorRutAsync(string rut);
    }
}
