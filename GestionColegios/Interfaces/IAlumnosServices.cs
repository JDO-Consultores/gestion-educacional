using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IAlumnosServices
    {
        Task<DataSourceResult> GetAlumnosIndexAsync(DataSourceRequest request);
        Task<AlumnoFichaViewModel> GetFichaAlumnoAsync(int alumnoId);
        Task<int> CreateAlumnoAsync(AlumnoFichaViewModel model, string createdBy);
        Task<int> UpdateAlumnoAsync(AlumnoFichaViewModel model, string modifiedBy);
        Task<int> RetirarAlumnoAsync(int alumnoId, int causalRetiroId, DateTime fechaRetiro, string observacion, string createdBy);
        Task<List<LogActividadViewModel>> GetHistorialAsync(int alumnoId);

        // Validación RUT
        Task<bool> RutExisteAsync(string rut, int excludeAlumnoId = 0);

        // Alergias
        Task<List<AlumnoAlergiaViewModel>> GetAlergiasAlumnoAsync(int alumnoId);
        Task<int> GuardarAlergiaAsync(AlumnoAlergiaViewModel model, string createdBy);
        Task<int> EliminarAlergiaAsync(int alergiaId, string modifiedBy);
        Task<(byte[] Contenido, string Nombre, string MimeType)> DescargarCertificadoAlergiaAsync(int alergiaId);

        // Discapacidades
        Task<List<AlumnoDiscapacidadViewModel>> GetDiscapacidadesAlumnoAsync(int alumnoId);
        Task<int> EliminarDiscapacidadAsync(int discapacidadId, string modifiedBy);
        Task<(byte[] Contenido, string Nombre, string MimeType)> DescargarCertificadoDiscapacidadAsync(int discapacidadId);

        // Cambio de RUT (extranjeros)
        Task<(int NuevoAlumnoId, string Error)> CambioRutAsync(CambioRutViewModel model, string createdBy);
        Task<TraspasoRutResumenViewModel> GetTraspasoRutAsync(int alumnoId);

        // Listas auxiliares
        Task<List<SelectItemViewModel>> GetEstadosAlumnoAsync();
        Task<List<int>> GetAniosEscolaresAlumnosAsync();
        Task<int> GetAnioActivoAsync();
    }
}
