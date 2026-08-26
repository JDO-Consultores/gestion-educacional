using GestionColegios.Interfaces;
using GestionColegios.Model;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class BaseServices
    {
        private protected readonly Entities _dbContext;
        private protected readonly IMapperService _mapperService;

        protected BaseServices(Entities dbContext, IMapperService mapperService)
        {
            _dbContext = dbContext;
            _dbContext.Database.CommandTimeout = 180;
            _mapperService = mapperService;
        }

        public string _JWTIssuer    => ConfigurationManager.AppSettings["JWT:Issuer"];
        public string _JWTSecretKey => ConfigurationManager.AppSettings["JWT:SecretKey"];
        public string _JWTAudience  => ConfigurationManager.AppSettings["JWT:Audience"];

        /// <summary>
        /// Devuelve el año escolar activo del sistema.
        /// Criterio: primero busca EsActivo=true; si no existe, usa el más reciente no cerrado.
        /// </summary>
        protected async Task<tbl_AnioEscolar> GetAnioEscolarActivoAsync()
        {
            // 1. Año marcado explícitamente como activo
            var activo = await _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive && a.EsActivo)
                .FirstOrDefaultAsync();

            if (activo != null) return activo;

            // 2. Fallback: el más reciente no cerrado
            return await _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive && !a.Cerrado)
                .OrderByDescending(a => a.Anio)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Registra una acción en el log de actividad vinculada a un alumno.
        /// </summary>
        protected async Task RegistrarLogAsync(
            string entidad,
            int alumnoId,
            string accion,
            string usuario,
            string detalle = null)
        {
            _dbContext.tbl_LogActividad.Add(new tbl_LogActividad
            {
                Entidad     = entidad,
                EntidadID   = alumnoId,
                Accion      = accion,
                Detalle     = detalle,
                Usuario     = usuario,
                FechaAccion = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
