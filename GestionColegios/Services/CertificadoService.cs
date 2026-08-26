using GestionColegios.Helpers;
using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GestionColegios.Services
{
    /// <summary>
    /// Generación on-demand de certificados Word y mantenedor de plantillas,
    /// firmantes y datos del establecimiento. Usa el contexto EDMX (Entities).
    /// </summary>
    public class CertificadoService : ICertificadoService
    {
        private readonly Entities _dbContext;
        private static readonly CultureInfo Cultura = new CultureInfo("es-CL");

        public CertificadoService(Entities dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.CommandTimeout = 180;
        }

        // ????????????????????????????????????????????????????????????????
        //  GENERACIÓN ON-DEMAND
        // ????????????????????????????????????????????????????????????????
        public async Task<(byte[] Contenido, string NombreArchivo)> GenerarCertificadoAsync(
            int plantillaId, int alumnoId, int? firmanteId)
        {
            var plantilla = await _dbContext.tbl_PlantillaCertificado
                .Include(p => p.tbl_FirmanteCertificado)
                .FirstOrDefaultAsync(p => p.ID == plantillaId && p.IsActive);

            if (plantilla == null || plantilla.Contenido == null || plantilla.Contenido.Length == 0)
                return (null, null);

            var idFirmante = firmanteId ?? plantilla.FirmanteDefectoID;
            var firmante = idFirmante.HasValue
                ? await _dbContext.tbl_FirmanteCertificado.FirstOrDefaultAsync(f => f.ID == idFirmante.Value)
                : null;

            var establecimiento = await _dbContext.tbl_Establecimiento
                .Include(e => e.tbl_Comuna.tbl_Region)
                .OrderBy(e => e.ID)
                .FirstOrDefaultAsync();

            var datos = await ConstruirReemplazosAsync(alumnoId, establecimiento, firmante);
            if (datos == null) return (null, null);

            var tieneLogo = establecimiento?.Logo != null && establecimiento.Logo.Length > 0;
            var bytes = WordTemplateEngine.Generar(
                plantilla.Contenido,
                datos,
                tieneLogo ? establecimiento.Logo : null);

            // Nombre de archivo limpio: CODIGO_RUT.docx (sin puntos ni separadores).
            var rutLimpio = (datos["ALUMNO_RUT"] ?? string.Empty)
                .Replace(".", "").Replace("/", "").Replace(" ", "");
            var nombreArchivo = $"{plantilla.Codigo}_{rutLimpio}.docx";

            return (bytes, nombreArchivo);
        }

        private async Task<Dictionary<string, string>> ConstruirReemplazosAsync(
            int alumnoId, tbl_Establecimiento est, tbl_FirmanteCertificado firmante)
        {
            var alumno = await _dbContext.tbl_Alumno
                .Include(a => a.tbl_AlumnoApoderado.Select(aa => aa.tbl_Apoderado))
                .Include(a => a.tbl_Matricula.Select(m => m.tbl_Curso.tbl_Grado))
                .Include(a => a.tbl_Matricula.Select(m => m.tbl_AnioEscolar))
                .FirstOrDefaultAsync(a => a.ID == alumnoId);

            if (alumno == null) return null;

            var matricula = alumno.tbl_Matricula
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.tbl_AnioEscolar.Anio)
                .FirstOrDefault();

            var apoderadoTitular = alumno.tbl_AlumnoApoderado
                .Where(aa => aa.IsActive && aa.EsApoderadoTitular && aa.tbl_Apoderado != null)
                .Select(aa => aa.tbl_Apoderado)
                .FirstOrDefault()
                ?? alumno.tbl_AlumnoApoderado
                    .Where(aa => aa.IsActive && aa.tbl_Apoderado != null)
                    .Select(aa => aa.tbl_Apoderado)
                    .FirstOrDefault();

            var hoy = DateTime.Now;

            string NombreCompletoAlumno() =>
                $"{alumno.Nombres} {alumno.ApellidoPaterno} {alumno.ApellidoMaterno}".Trim();

            string NombreCompletoApoderado() => apoderadoTitular == null
                ? string.Empty
                : $"{apoderadoTitular.Nombres} {apoderadoTitular.ApellidoPaterno} {apoderadoTitular.ApellidoMaterno}".Trim();

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Establecimiento / membrete
                ["COLEGIO_NOMBRE"]    = est?.Nombre ?? string.Empty,
                ["COLEGIO_DIRECCION"] = est?.Direccion ?? string.Empty,
                ["COLEGIO_CIUDAD"]    = est?.Ciudad ?? string.Empty,
                ["COLEGIO_COMUNA"]    = est?.tbl_Comuna?.Nombre ?? est?.Ciudad ?? string.Empty,
                ["COLEGIO_REGION"]    = est?.tbl_Comuna?.tbl_Region?.Nombre ?? string.Empty,
                ["COLEGIO_TELEFONO"]  = est?.Telefono ?? string.Empty,
                ["COLEGIO_RBD"]       = est?.RBD ?? string.Empty,
                ["COLEGIO_EMAIL"]     = est?.Email ?? string.Empty,
                ["COLEGIO_WEB"]       = est?.SitioWeb ?? string.Empty,

                // Alumno
                ["ALUMNO_NOMBRE"] = NombreCompletoAlumno().ToUpper(Cultura),
                ["ALUMNO_RUT"]    = RutHelper.Formatear(alumno.Rut),
                ["ALUMNO_CURSO"]  = matricula?.tbl_Curso != null
                    ? $"{matricula.tbl_Curso.tbl_Grado?.Nombre} {matricula.tbl_Curso.Letra}".Trim()
                    : string.Empty,
                ["ALUMNO_ANIO"]   = matricula?.tbl_AnioEscolar?.Anio.ToString() ?? string.Empty,

                // Apoderado titular
                ["APODERADO_NOMBRE"] = NombreCompletoApoderado().ToUpper(Cultura),
                ["APODERADO_RUT"]    = apoderadoTitular != null
                    ? RutHelper.Formatear(apoderadoTitular.Rut)
                    : string.Empty,

                // Fecha / firmante
                ["FECHA"]           = hoy.ToString("dddd, MMMM dd, yyyy", Cultura),
                ["ANIO_ACTUAL"]     = hoy.Year.ToString(),
                ["FIRMANTE_NOMBRE"] = firmante?.Nombre ?? string.Empty,
                ["FIRMANTE_CARGO"]  = firmante?.Cargo ?? string.Empty
            };
        }

        public async Task<List<PlantillaCertificadoItemViewModel>> GetPlantillasDisponiblesAsync()
        {
            return await _dbContext.tbl_PlantillaCertificado
                .Where(p => p.IsActive && p.Contenido != null)
                .OrderBy(p => p.Nombre)
                .Select(MapPlantilla())
                .ToListAsync();
        }

        // ????????????????????????????????????????????????????????????????
        //  MANTENEDOR — PLANTILLAS
        // ????????????????????????????????????????????????????????????????
        public async Task<List<PlantillaCertificadoItemViewModel>> GetPlantillasAsync()
        {
            return await _dbContext.tbl_PlantillaCertificado
                .OrderBy(p => p.Nombre)
                .Select(MapPlantilla())
                .ToListAsync();
        }

        private static System.Linq.Expressions.Expression<Func<tbl_PlantillaCertificado, PlantillaCertificadoItemViewModel>> MapPlantilla()
        {
            return p => new PlantillaCertificadoItemViewModel
            {
                ID                = p.ID,
                Codigo            = p.Codigo,
                Nombre            = p.Nombre,
                Descripcion       = p.Descripcion,
                NombreArchivo     = p.NombreArchivo,
                TienePlantilla    = p.Contenido != null,
                FirmanteDefectoID = p.FirmanteDefectoID,
                FirmanteDefecto   = p.tbl_FirmanteCertificado != null
                    ? p.tbl_FirmanteCertificado.Nombre + " — " + p.tbl_FirmanteCertificado.Cargo
                    : null,
                IsActive          = p.IsActive
            };
        }

        public async Task<bool> SubirPlantillaAsync(int plantillaId, HttpPostedFileBase archivo, string usuario)
        {
            if (archivo == null || archivo.ContentLength == 0) return false;

            var ext = Path.GetExtension(archivo.FileName)?.ToLowerInvariant();
            if (ext != ".docx") return false;

            byte[] contenido;
            using (var stream = new MemoryStream())
            {
                archivo.InputStream.CopyTo(stream);
                contenido = stream.ToArray();
            }

            var plantilla = await _dbContext.tbl_PlantillaCertificado.FindAsync(plantillaId);
            if (plantilla == null) return false;

            plantilla.Contenido     = contenido;
            plantilla.NombreArchivo = Path.GetFileName(archivo.FileName);
            plantilla.ModifiedDate  = DateTime.Now;
            plantilla.ModifiedBy    = usuario;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<(byte[] Contenido, string NombreArchivo)> DescargarPlantillaAsync(int plantillaId)
        {
            var plantilla = await _dbContext.tbl_PlantillaCertificado.FindAsync(plantillaId);
            if (plantilla?.Contenido == null || plantilla.Contenido.Length == 0)
                return (null, null);

            var nombre = !string.IsNullOrWhiteSpace(plantilla.NombreArchivo)
                ? plantilla.NombreArchivo
                : $"{plantilla.Codigo}.docx";

            return (plantilla.Contenido, nombre);
        }

        public async Task<bool> SetFirmanteDefectoAsync(int plantillaId, int? firmanteId, string usuario)
        {
            var plantilla = await _dbContext.tbl_PlantillaCertificado.FindAsync(plantillaId);
            if (plantilla == null) return false;

            plantilla.FirmanteDefectoID = firmanteId;
            plantilla.ModifiedDate      = DateTime.Now;
            plantilla.ModifiedBy        = usuario;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetPlantillaActivaAsync(int plantillaId, bool activa, string usuario)
        {
            var plantilla = await _dbContext.tbl_PlantillaCertificado.FindAsync(plantillaId);
            if (plantilla == null) return false;

            plantilla.IsActive     = activa;
            plantilla.ModifiedDate = DateTime.Now;
            plantilla.ModifiedBy   = usuario;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // ????????????????????????????????????????????????????????????????
        //  MANTENEDOR — FIRMANTES
        // ????????????????????????????????????????????????????????????????
        public async Task<List<FirmanteItemViewModel>> GetFirmantesAsync(bool soloActivos = false)
        {
            var query = _dbContext.tbl_FirmanteCertificado.AsQueryable();
            if (soloActivos) query = query.Where(f => f.IsActive);

            return await query
                .OrderBy(f => f.Nombre)
                .Select(f => new FirmanteItemViewModel
                {
                    ID       = f.ID,
                    Nombre   = f.Nombre,
                    Cargo    = f.Cargo,
                    IsActive = f.IsActive
                })
                .ToListAsync();
        }

        public async Task<int> GuardarFirmanteAsync(FirmanteGuardarViewModel model, string usuario)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Cargo))
                return -1;

            tbl_FirmanteCertificado firmante;
            if (model.ID > 0)
            {
                firmante = await _dbContext.tbl_FirmanteCertificado.FindAsync(model.ID);
                if (firmante == null) return -1;
                firmante.ModifiedDate = DateTime.Now;
                firmante.ModifiedBy   = usuario;
            }
            else
            {
                firmante = new tbl_FirmanteCertificado
                {
                    IsActive    = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy   = usuario
                };
                _dbContext.tbl_FirmanteCertificado.Add(firmante);
            }

            firmante.Nombre   = model.Nombre.Trim();
            firmante.Cargo    = model.Cargo.Trim();
            firmante.IsActive = model.IsActive;

            await _dbContext.SaveChangesAsync();
            return firmante.ID;
        }

        public async Task<bool> EliminarFirmanteAsync(int firmanteId, string usuario)
        {
            var firmante = await _dbContext.tbl_FirmanteCertificado.FindAsync(firmanteId);
            if (firmante == null) return false;

            // Soft-delete para no romper plantillas que lo referencien.
            firmante.IsActive     = false;
            firmante.ModifiedDate = DateTime.Now;
            firmante.ModifiedBy   = usuario;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // ????????????????????????????????????????????????????????????????
        //  MANTENEDOR — ESTABLECIMIENTO
        // ????????????????????????????????????????????????????????????????
        public async Task<EstablecimientoViewModel> GetEstablecimientoAsync()
        {
            var est = await _dbContext.tbl_Establecimiento
                .Include(e => e.tbl_Comuna.tbl_Region)
                .OrderBy(e => e.ID)
                .FirstOrDefaultAsync();
            if (est == null) return new EstablecimientoViewModel();

            return new EstablecimientoViewModel
            {
                ID        = est.ID,
                RBD       = est.RBD,
                Nombre    = est.Nombre,
                Direccion = est.Direccion,
                ComunaID  = est.ComunaID,
                RegionID  = est.tbl_Comuna != null ? est.tbl_Comuna.RegionID : (int?)null,
                Ciudad    = est.tbl_Comuna != null ? est.tbl_Comuna.Nombre : est.Ciudad,
                Telefono  = est.Telefono,
                Email     = est.Email,
                SitioWeb  = est.SitioWeb,
                TieneLogo = est.Logo != null && est.Logo.Length > 0
            };
        }

        public async Task<bool> GuardarEstablecimientoAsync(EstablecimientoViewModel model, string usuario)
        {
            if (model == null) return false;

            var est = model.ID > 0
                ? await _dbContext.tbl_Establecimiento.FindAsync(model.ID)
                : await _dbContext.tbl_Establecimiento.OrderBy(e => e.ID).FirstOrDefaultAsync();

            // Si aun no existe registro de establecimiento, se crea uno.
            if (est == null)
            {
                est = new tbl_Establecimiento
                {
                    IsActive    = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy   = usuario
                };
                _dbContext.tbl_Establecimiento.Add(est);
            }

            est.RBD       = Truncar(model.RBD, 10);
            est.Nombre    = Truncar(model.Nombre, 150);
            est.Direccion = Truncar(model.Direccion, 200);
            est.ComunaID  = model.ComunaID;
            est.Telefono  = Truncar(model.Telefono, 20);
            est.Email     = Truncar(model.Email, 100);
            est.SitioWeb  = Truncar(model.SitioWeb, 200);

            // Sincroniza el texto de ciudad (usado en el membrete) con la comuna seleccionada.
            if (model.ComunaID.HasValue)
            {
                var comuna = await _dbContext.tbl_Comuna.FindAsync(model.ComunaID.Value);
                est.Ciudad = Truncar(comuna?.Nombre, 200);
            }
            else
            {
                est.Ciudad = null;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static string Truncar(string valor, int max)
        {
            if (string.IsNullOrEmpty(valor)) return valor;
            valor = valor.Trim();
            return valor.Length > max ? valor.Substring(0, max) : valor;
        }

        public async Task<bool> SubirLogoAsync(HttpPostedFileBase archivo, string usuario)
        {
            if (archivo == null || archivo.ContentLength == 0) return false;

            var ext = Path.GetExtension(archivo.FileName)?.ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg") return false;

            byte[] contenido;
            using (var stream = new MemoryStream())
            {
                archivo.InputStream.CopyTo(stream);
                contenido = stream.ToArray();
            }

            var est = await _dbContext.tbl_Establecimiento.OrderBy(e => e.ID).FirstOrDefaultAsync();
            if (est == null) return false;

            est.Logo         = contenido;
            est.LogoMimeType = ext == ".png" ? "image/png" : "image/jpeg";

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<(byte[] Contenido, string MimeType)> GetLogoAsync()
        {
            var est = await _dbContext.tbl_Establecimiento
                .OrderBy(e => e.ID)
                .FirstOrDefaultAsync();

            if (est?.Logo == null || est.Logo.Length == 0) return (null, null);
            return (est.Logo, est.LogoMimeType ?? "image/png");
        }
    }
}
