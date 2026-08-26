using GestionColegios.Helpers;
using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class ApoderadoService : BaseServices, IApoderadoService
    {
        public ApoderadoService(Entities dbContext, IMapperService mapperService)
            : base(dbContext, mapperService) { }

        public async Task<ApoderadoFormViewModel> GetFormDataAsync(int alumnoId, int? alumnoApoderadoId = null)
        {
            var alumno = await _dbContext.tbl_Alumno.FindAsync(alumnoId);
            if (alumno == null) return null;

            var parentescos = await _dbContext.tbl_Parentesco
                .Where(p => p.IsActive)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectItemViewModel { ID = p.ID, Texto = p.Nombre })
                .ToListAsync();

            var vm = new ApoderadoFormViewModel
            {
                AlumnoID             = alumnoId,
                AlumnoNombreCompleto = alumno.ApellidoPaterno + " " + alumno.ApellidoMaterno + ", " + alumno.Nombres,
                AlumnoRut            = RutHelper.Formatear(alumno.Rut),
                Parentescos          = parentescos
            };

            // Modo edición: cargar vínculo existente
            if (alumnoApoderadoId.HasValue)
            {
                var vinculo = await _dbContext.tbl_AlumnoApoderado
                    .Include(aa => aa.tbl_Apoderado.tbl_Nacionalidad)
                    .Include(aa => aa.tbl_Apoderado.tbl_NivelEducacional)
                    .Include(aa => aa.tbl_Apoderado.tbl_SituacionLaboral)
                    .Include(aa => aa.tbl_Apoderado.tbl_Comuna.tbl_Region)
                    .FirstOrDefaultAsync(aa => aa.ID == alumnoApoderadoId && aa.AlumnoID == alumnoId);

                if (vinculo != null)
                {
                    vm.ParentescoID       = vinculo.ParentescoID;
                    vm.EsApoderadoTitular = vinculo.EsApoderadoTitular;
                    vm.TipoApoderado      = vinculo.EsPadre ? "Padre" : vinculo.EsMadre ? "Madre" : "Apoderado";
                    var a = vinculo.tbl_Apoderado;
                    vm.Apoderado = MapApoderado(a);
                    vm.Apoderado.ID = a.ID;
                }
            }

            return vm;
        }

        public async Task<int> GuardarApoderadoAsync(ApoderadoFormViewModel model, string createdBy)
        {
            var ap = model.Apoderado;
            var rutLimpio = RutHelper.Limpiar(ap.Rut);

            tbl_Apoderado entidad;

            if (ap.ID > 0)
            {
                // Editar apoderado existente
                entidad = await _dbContext.tbl_Apoderado.FindAsync(ap.ID);
                if (entidad == null) return 0;
            }
            else
            {
                // ¿Ya existe un apoderado con ese RUT?
                entidad = await _dbContext.tbl_Apoderado
                    .FirstOrDefaultAsync(x => x.Rut == rutLimpio && x.IsActive);

                if (entidad == null)
                {
                    entidad = new tbl_Apoderado
                    {
                        IsActive    = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy   = createdBy
                    };
                    _dbContext.tbl_Apoderado.Add(entidad);
                }
            }

            // Actualizar datos del apoderado
            entidad.Rut                = rutLimpio;
            entidad.Nombres            = ap.Nombres;
            entidad.ApellidoPaterno    = ap.ApellidoPaterno;
            entidad.ApellidoMaterno    = ap.ApellidoMaterno;
            entidad.NacionalidadID     = ap.NacionalidadID;
            entidad.NivelEducacionalID = ap.NivelEducacionalID;
            entidad.SituacionLaboralID = ap.SituacionLaboralID;
            entidad.LugarTrabajo       = ap.LugarTrabajo;
            entidad.Direccion          = ap.Direccion;
            entidad.ComunaID           = ap.ComunaID;
            entidad.Telefono           = ap.Telefono;
            entidad.TelefonoCelular    = ap.TelefonoCelular;
            entidad.Email              = ap.Email;

            await _dbContext.SaveChangesAsync();

            // Crear o actualizar vínculo alumno ? apoderado
            var vinculo = await _dbContext.tbl_AlumnoApoderado
                .FirstOrDefaultAsync(aa => aa.AlumnoID == model.AlumnoID
                                        && aa.ApoderadoID == entidad.ID
                                        && aa.IsActive);

            if (vinculo == null)
            {
                // Si se marca como titular, quitar el titular actual
                if (model.EsApoderadoTitular)
                    await QuitarTitularActualAsync(model.AlumnoID);

                vinculo = new tbl_AlumnoApoderado
                {
                    AlumnoID    = model.AlumnoID,
                    ApoderadoID = entidad.ID,
                    IsActive    = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy   = createdBy
                };
                _dbContext.tbl_AlumnoApoderado.Add(vinculo);
            }
            else if (model.EsApoderadoTitular && !vinculo.EsApoderadoTitular)
            {
                await QuitarTitularActualAsync(model.AlumnoID);
            }

            vinculo.ParentescoID       = model.ParentescoID;
            vinculo.EsApoderadoTitular = model.EsApoderadoTitular;
            vinculo.EsPadre            = model.TipoApoderado == "Padre";
            vinculo.EsMadre            = model.TipoApoderado == "Madre";

            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Apoderado", model.AlumnoID,
                ap.ID > 0 ? "EDITÓ APODERADO" : "REGISTRÓ APODERADO",
                createdBy,
                $"{entidad.ApellidoPaterno} {entidad.ApellidoMaterno}, {entidad.Nombres} | RUT: {RutHelper.Formatear(entidad.Rut)}");

            return entidad.ID;
        }

        public async Task<int> DesvincularApoderadoAsync(int alumnoApoderadoId, string modifiedBy)
        {
            var vinculo = await _dbContext.tbl_AlumnoApoderado.FindAsync(alumnoApoderadoId);
            if (vinculo == null) return 0;

            vinculo.IsActive = false;
            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Apoderado", vinculo.AlumnoID, "DESVINCULÓ APODERADO",
                modifiedBy, $"AlumnoApoderadoID: {alumnoApoderadoId}");
            return alumnoApoderadoId;
        }

        public async Task<ApoderadoViewModel> BuscarPorRutAsync(string rut)
        {
            var limpio = RutHelper.Limpiar(rut);
            var a = await _dbContext.tbl_Apoderado
                .Include(x => x.tbl_Nacionalidad)
                .Include(x => x.tbl_NivelEducacional)
                .Include(x => x.tbl_SituacionLaboral)
                .Include(x => x.tbl_Comuna.tbl_Region)
                .FirstOrDefaultAsync(x => x.Rut == limpio && x.IsActive);

            return a == null ? null : MapApoderado(a);
        }

        // ?? Helpers ???????????????????????????????????????????????????????
        private static ApoderadoViewModel MapApoderado(tbl_Apoderado a)
        {
            return new ApoderadoViewModel
            {
                ID                 = a.ID,
                Rut                = RutHelper.Formatear(a.Rut),
                Nombres            = a.Nombres,
                ApellidoPaterno    = a.ApellidoPaterno,
                ApellidoMaterno    = a.ApellidoMaterno,
                NacionalidadID     = a.NacionalidadID,
                Nacionalidad       = a.tbl_Nacionalidad?.Nombre,
                NivelEducacionalID = a.NivelEducacionalID,
                NivelEducacional   = a.tbl_NivelEducacional?.Nombre,
                SituacionLaboralID = a.SituacionLaboralID,
                SituacionLaboral   = a.tbl_SituacionLaboral?.Nombre,
                LugarTrabajo       = a.LugarTrabajo,
                Direccion          = a.Direccion,
                ComunaID           = a.ComunaID,
                RegionID           = a.tbl_Comuna?.RegionID,
                Comuna             = a.tbl_Comuna?.Nombre,
                Region             = a.tbl_Comuna?.tbl_Region?.Nombre,
                Telefono           = a.Telefono,
                TelefonoCelular    = a.TelefonoCelular,
                Email              = a.Email
            };
        }

        private async Task QuitarTitularActualAsync(int alumnoId)
        {
            var titular = await _dbContext.tbl_AlumnoApoderado
                .FirstOrDefaultAsync(aa => aa.AlumnoID == alumnoId
                                        && aa.EsApoderadoTitular
                                        && aa.IsActive);
            if (titular != null)
                titular.EsApoderadoTitular = false;
        }
    }
}
