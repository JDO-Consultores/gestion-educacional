using GestionColegios.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IHomeService
    {
        Task<DashboardViewModel> GetDashboardStatsAsync();
        Task<List<AlumnosPorCursoViewModel>> GetAlumnosPorCursoAsync(int anioEscolarId);
        Task<List<ActividadRecienteViewModel>> GetActividadRecienteAsync(int cantidad);
    }
}
