using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IMatriculaService
    {
        Task<DataSourceResult> GetMatriculasPorAlumnoAsync(int alumnoId, DataSourceRequest request);
        Task<DataSourceResult> GetMatriculasAsync(int? anioEscolarId, DataSourceRequest request);
        Task<MatriculaResumenAnioViewModel> GetResumenAnioAsync(int? anioEscolarId);
        Task<MatriculaViewModel> GetMatriculaAsync(int matriculaId);
        Task<MatriculaFormViewModel> GetFormDataAsync(int alumnoId);
        Task<MatriculaResultado> CreateMatriculaAsync(MatriculaViewModel model, string createdBy);
        Task<int> UpdateMatriculaAsync(MatriculaViewModel model, string modifiedBy);
        Task<int> AnularMatriculaAsync(int matriculaId, string observacion, string modifiedBy);
        Task<bool> ExisteMatriculaEnAnioAsync(int alumnoId, int anioEscolarId);
        Task<bool> ActualizarEstadoSegunDocumentosAsync(int alumnoId, int anioEscolarId, string modifiedBy);

        // Lista de espera
        Task<int> CorrerListaEsperaAsync(int cursoId, string modifiedBy);

        // Listas auxiliares
        Task<List<SelectItemViewModel>> GetEstadosMatriculaAsync();
    }
}