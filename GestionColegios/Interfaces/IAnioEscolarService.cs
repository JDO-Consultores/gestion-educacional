using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IAnioEscolarService
    {
        // Años escolares
        Task<DataSourceResult> GetAniosEscolaresAsync(DataSourceRequest request);
        Task<AnioEscolarDetalleViewModel> GetDetalleAsync(int id);
        Task<AnioEscolarFormViewModel> GetFormAnioAsync(int id = 0);
        Task<int> CreateAnioEscolarAsync(AnioEscolarFormViewModel model, string usuario);
        Task<int> UpdateAnioEscolarAsync(AnioEscolarFormViewModel model, string usuario);
        Task<int> CerrarReobrirAnioAsync(int id, string usuario);
        Task<int> MarcarComoActivoAsync(int id, string usuario);

        // Cierre de año: promoción de alumnos
        Task<DataSourceResult> GetPromocionAlumnosAsync(int anioEscolarId, DataSourceRequest request);
        Task<int> RegistrarPromocionAsync(RegistrarPromocionViewModel model, string usuario);
        Task<int> PromoverCursoAsync(int anioEscolarId, int cursoId, string usuario);
        Task<List<SelectItemViewModel>> GetCursosConPendientesAsync(int anioEscolarId);
        Task<(bool Ok, string Error)> AutorizarMatriculaCanceladaAsync(AutorizarMatriculaViewModel model, string usuario);

        // Cursos
        Task<CursoFormViewModel> GetFormCursoAsync(int anioEscolarId, int cursoId = 0);
        Task<int> CreateCursoAsync(CursoFormViewModel model, string usuario);
        Task<int> UpdateCursoAsync(CursoFormViewModel model, string usuario);
        Task<int> EliminarCursoAsync(int cursoId, string usuario);

        // Lookups
        Task<List<SelectItemViewModel>> GetProfesoresAsync();
        Task<List<SelectItemViewModel>> GetGradosAsync();
    }
}
