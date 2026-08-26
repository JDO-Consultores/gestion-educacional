using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class ProfesorJefeService : BaseServices, IProfesorJefeService
    {
        public ProfesorJefeService(Entities dbContext, IMapperService mapperService)
            : base(dbContext, mapperService) { }

        public async Task<DataSourceResult> GetProfesoresAsync(DataSourceRequest request)
        {
            var query = _dbContext.tbl_ProfesorJefe
                .Where(p => p.IsActive)
                .Select(p => new ProfesorJefeViewModel
                {
                    ID              = p.ID,
                    Rut             = p.Rut,
                    Nombre          = p.Nombre,
                    Apellido        = p.Apellido,
                    Email           = p.Email,
                    Telefono        = p.Telefono,
                    EstadoProfesorID = p.EstadoProfesorID,
                    EstadoProfesor  = p.tbl_EstadoProfesor.Nombre,
                    CursosAsignados = _dbContext.tbl_Curso
                        .Count(c => c.ProfesorJefeID == p.ID && c.IsActive)
                });

            return await query
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToDataSourceResultAsync(
                    request.Take, request.Skip,
                    request.Sort, request.Filter,
                    request.Aggregate, request.Group);
        }

        public async Task<ProfesorJefeViewModel> GetByIdAsync(int id)
        {
            var p = await _dbContext.tbl_ProfesorJefe
                .FirstOrDefaultAsync(x => x.ID == id && x.IsActive);
            if (p == null) return null;

            return new ProfesorJefeViewModel
            {
                ID               = p.ID,
                Rut              = p.Rut,
                Nombre           = p.Nombre,
                Apellido         = p.Apellido,
                Email            = p.Email,
                Telefono         = p.Telefono,
                EstadoProfesorID = p.EstadoProfesorID,
                EstadoProfesor   = p.tbl_EstadoProfesor?.Nombre,
                Vigente          = p.IsActive,
                EstadosDisponibles = await GetEstadosProfesorAsync()
            };
        }

        public async Task<int> CreateAsync(ProfesorJefeViewModel model, string usuario)
        {
            if (await RutExisteAsync(model.Rut))
                return -1;

            var entidad = new tbl_ProfesorJefe
            {
                Rut              = model.Rut?.Replace(".", "").Trim().ToUpper(),
                Nombre           = model.Nombre,
                Apellido         = model.Apellido,
                Email            = model.Email,
                Telefono         = model.Telefono,
                EstadoProfesorID = model.EstadoProfesorID > 0 ? model.EstadoProfesorID : 1,
                IsActive         = true,
                CreatedDate      = DateTime.UtcNow,
                CreatedBy        = usuario
            };

            _dbContext.tbl_ProfesorJefe.Add(entidad);
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> UpdateAsync(ProfesorJefeViewModel model, string usuario)
        {
            var entidad = await _dbContext.tbl_ProfesorJefe.FindAsync(model.ID);
            if (entidad == null) return 0;

            if (await RutExisteAsync(model.Rut, model.ID))
                return -1;

            entidad.Rut              = model.Rut?.Replace(".", "").Trim().ToUpper();
            entidad.Nombre           = model.Nombre;
            entidad.Apellido         = model.Apellido;
            entidad.Email            = model.Email;
            entidad.Telefono         = model.Telefono;
            entidad.EstadoProfesorID = model.EstadoProfesorID;
            entidad.IsActive         = model.Vigente;
            entidad.ModifiedDate     = DateTime.UtcNow;
            entidad.ModifiedBy       = usuario;

            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<bool> RutExisteAsync(string rut, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(rut)) return false;
            var rutNorm = rut.Replace(".", "").Trim().ToUpper();
            return await _dbContext.tbl_ProfesorJefe
                .AnyAsync(p => p.IsActive
                            && p.Rut.ToUpper() == rutNorm
                            && p.ID != excludeId);
        }

        public async Task<List<SelectItemViewModel>> GetEstadosProfesorAsync()
        {
            return await _dbContext.tbl_EstadoProfesor
                .Where(e => e.IsActive)
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToListAsync();
        }
    }
}
