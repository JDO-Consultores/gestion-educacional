using GestionColegios.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace GestionColegios.Interfaces
{
    public interface ICertificadoService
    {
        // ?? Generación on-demand ?????????????????????????????????????????
        /// <summary>
        /// Genera un certificado en Word (.docx) para un alumno usando la plantilla
        /// indicada y, opcionalmente, un firmante distinto al de defecto.
        /// </summary>
        Task<(byte[] Contenido, string NombreArchivo)> GenerarCertificadoAsync(
            int plantillaId, int alumnoId, int? firmanteId);

        /// <summary>Plantillas disponibles para generar (activas y con archivo cargado).</summary>
        Task<List<PlantillaCertificadoItemViewModel>> GetPlantillasDisponiblesAsync();

        // ?? Mantenedor: Plantillas ???????????????????????????????????????
        Task<List<PlantillaCertificadoItemViewModel>> GetPlantillasAsync();
        Task<bool> SubirPlantillaAsync(int plantillaId, HttpPostedFileBase archivo, string usuario);
        Task<(byte[] Contenido, string NombreArchivo)> DescargarPlantillaAsync(int plantillaId);
        Task<bool> SetFirmanteDefectoAsync(int plantillaId, int? firmanteId, string usuario);
        Task<bool> SetPlantillaActivaAsync(int plantillaId, bool activa, string usuario);

        // ?? Mantenedor: Firmantes ????????????????????????????????????????
        Task<List<FirmanteItemViewModel>> GetFirmantesAsync(bool soloActivos = false);
        Task<int> GuardarFirmanteAsync(FirmanteGuardarViewModel model, string usuario);
        Task<bool> EliminarFirmanteAsync(int firmanteId, string usuario);

        // ?? Mantenedor: Establecimiento ??????????????????????????????????
        Task<EstablecimientoViewModel> GetEstablecimientoAsync();
        Task<bool> GuardarEstablecimientoAsync(EstablecimientoViewModel model, string usuario);
        Task<bool> SubirLogoAsync(HttpPostedFileBase archivo, string usuario);
        Task<(byte[] Contenido, string MimeType)> GetLogoAsync();
    }
}
