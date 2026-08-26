using GestionColegios.Helpers;
using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using KendoNET.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class AlumnosService : BaseServices, IAlumnosServices
    {
        public AlumnosService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<DataSourceResult> GetAlumnosIndexAsync(DataSourceRequest request)
        {
            // Join against the most-recent active matricula once to avoid two correlated subqueries per row.
            var ultimaMatricula = _dbContext.tbl_Matricula
                .Where(m => m.IsActive)
                .GroupBy(m => m.AlumnoID)
                .Select(g => g.OrderByDescending(m => m.tbl_AnioEscolar.Anio).FirstOrDefault());

            var query = _dbContext.tbl_Alumno
                .Where(a => a.IsActive)
                .GroupJoin(
                    ultimaMatricula,
                    a => a.ID,
                    m => m.AlumnoID,
                    (a, ms) => new AlumnoListadoViewModel
                    {
                        ID = a.ID,
                        Rut = a.Rut,
                        NombreCompleto = a.ApellidoPaterno + " " + a.ApellidoMaterno + ", " + a.Nombres,
                        EstadoAlumno = a.tbl_EstadoAlumno.Nombre,
                        TienePIE = a.TienePIE,
                        FotoUrl = a.FotoContenido != null ? "/Documento/FotoAlumno/" + a.ID : null,
                        Curso = ms.Select(m => m.tbl_Curso.tbl_Grado.Nombre + " " + m.tbl_Curso.Letra).FirstOrDefault(),
                        AnioEscolar = ms.Select(m => (int?)m.tbl_AnioEscolar.Anio).FirstOrDefault(),
                        NroMatricula = ms.Select(m => m.NroMatricula).FirstOrDefault()
                    });

            var result = await query
                .OrderBy(x => x.NombreCompleto)
                .ToDataSourceResultAsync(
                    request.Take,
                    request.Skip,
                    request.Sort,
                    request.Filter,
                    request.Aggregate,
                    request.Group);

            // Formatear RUT en memoria (no traducible a SQL)
            if (result.Data != null)
            {
                foreach (var item in result.Data.Cast<AlumnoListadoViewModel>())
                    item.Rut = RutHelper.Formatear(item.Rut);
            }

            return result;
        }

        public async Task<AlumnoFichaViewModel> GetFichaAlumnoAsync(int alumnoId)
        {
            var alumno = await _dbContext.tbl_Alumno
                .Include(a => a.tbl_Sexo)
                .Include(a => a.tbl_Nacionalidad)
                .Include(a => a.tbl_Comuna.tbl_Region)
                .Include(a => a.tbl_SistemaSalud)
                .Include(a => a.tbl_EstadoAlumno)
                .Include(a => a.tbl_Etnia)
                .Include(a => a.tbl_CondicionSocioeconomica)
                .Include(a => a.tbl_ViveCon)
                .Include(a => a.tbl_AlumnoApoderado.Select(aa => aa.tbl_Apoderado))
                .Include(a => a.tbl_AlumnoApoderado.Select(aa => aa.tbl_Apoderado.tbl_NivelEducacional))
                .Include(a => a.tbl_AlumnoApoderado.Select(aa => aa.tbl_Apoderado.tbl_SituacionLaboral))
                .Include(a => a.tbl_AlumnoApoderado.Select(aa => aa.tbl_Parentesco))
                .Include(a => a.tbl_DocumentoAlumno.Select(d => d.tbl_TipoDocumento))
                .Include(a => a.tbl_AlumnoAlergia.Select(al => al.tbl_TipoAlergia))
                .Include(a => a.tbl_AlumnoDiscapacidad.Select(d => d.tbl_TipoDiscapacidad))
                .Include(a => a.tbl_Matricula.Select(m => m.tbl_Curso.tbl_Grado))
                .Include(a => a.tbl_Matricula.Select(m => m.tbl_AnioEscolar))
                .Include(a => a.tbl_RetiroAlumno.Select(r => r.tbl_CausalRetiro))
                .FirstOrDefaultAsync(a => a.ID == alumnoId);

            if (alumno == null) return null;

            var matriculaVigente = alumno.tbl_Matricula
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.tbl_AnioEscolar.Anio)
                .FirstOrDefault();

            return new AlumnoFichaViewModel
            {
                ID = alumno.ID,
                Rut = RutHelper.Formatear(alumno.Rut),
                Nombres = alumno.Nombres,
                ApellidoPaterno = alumno.ApellidoPaterno,
                ApellidoMaterno = alumno.ApellidoMaterno,
                FechaNacimiento = alumno.FechaNacimiento,
                SexoID = alumno.SexoID,
                Sexo = alumno.tbl_Sexo?.Nombre,
                NacionalidadID = alumno.NacionalidadID,
                Nacionalidad = alumno.tbl_Nacionalidad?.Nombre,
                Direccion = alumno.Direccion,
                ComunaID = alumno.ComunaID,
                Comuna = alumno.tbl_Comuna?.Nombre,
                RegionID = alumno.tbl_Comuna?.RegionID,
                Region = alumno.tbl_Comuna?.tbl_Region?.Nombre,
                Telefono = alumno.Telefono,
                Email = alumno.Email,
                SistemaSaludID = alumno.SistemaSaludID,
                SistemaSalud = alumno.tbl_SistemaSalud?.Nombre,
                Alergias = alumno.tbl_AlumnoAlergia
                    .Where(al => al.IsActive)
                    .Select(al => new AlumnoAlergiaViewModel
                    {
                        ID = al.ID,
                        AlumnoID = al.AlumnoID,
                        TipoAlergiaID = al.TipoAlergiaID,
                        TipoAlergia = al.tbl_TipoAlergia.Nombre,
                        NombreAlergia = al.NombreAlergia,
                        Descripcion = al.Descripcion,
                        CertificadoNombre = al.CertificadoNombre,
                        CertificadoMimeType = al.CertificadoMimeType
                    }).ToList(),
                TienePIE = alumno.TienePIE,
                Discapacidades = alumno.tbl_AlumnoDiscapacidad
                    .Where(d => d.IsActive)
                    .Select(d => new AlumnoDiscapacidadViewModel
                    {
                        ID = d.ID,
                        AlumnoID = d.AlumnoID,
                        TipoDiscapacidadID = d.TipoDiscapacidadID,
                        TipoDiscapacidad = d.tbl_TipoDiscapacidad.Nombre,
                        Descripcion = d.Descripcion,
                        CertificadoNombre = d.CertificadoNombre,
                        CertificadoMimeType = d.CertificadoMimeType
                    }).ToList(),
                EtniaID = alumno.EtniaID,
                Etnia = alumno.tbl_Etnia?.Nombre,
                CondicionSocioeconomicaID = alumno.CondicionSocioeconomicaID,
                CondicionSocioeconomica = alumno.tbl_CondicionSocioeconomica?.Nombre,
                ViveConID = alumno.ViveConID,
                ViveCon = alumno.tbl_ViveCon?.Nombre,
                AlumnoOrigenID = alumno.AlumnoOrigenID,
                RutAnterior = !string.IsNullOrEmpty(alumno.RutAnterior) ? RutHelper.Formatear(alumno.RutAnterior) : null,
                EstadoAlumnoID = alumno.EstadoAlumnoID,
                EstadoAlumno = alumno.tbl_EstadoAlumno?.Nombre,
                FueTraspasado = !alumno.IsActive && _dbContext.tbl_TraspasoRut.Any(t => t.AlumnoOrigenID == alumno.ID),
                AlumnoDestinoID = _dbContext.tbl_TraspasoRut
                    .Where(t => t.AlumnoOrigenID == alumno.ID)
                    .Select(t => (int?)t.AlumnoDestinoID)
                    .FirstOrDefault(),
                RutNuevo = _dbContext.tbl_TraspasoRut
                    .Where(t => t.AlumnoOrigenID == alumno.ID)
                    .Select(t => t.RutNuevo)
                    .FirstOrDefault(),
                FechaRetiro = alumno.tbl_RetiroAlumno
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.FechaRetiro)
                    .Select(r => (DateTime?)r.FechaRetiro)
                    .FirstOrDefault(),
                CausalRetiro = alumno.tbl_RetiroAlumno
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.FechaRetiro)
                    .Select(r => r.tbl_CausalRetiro.Nombre)
                    .FirstOrDefault(),
                ApoderadoPadre = alumno.tbl_AlumnoApoderado
                    .Where(aa => aa.IsActive && aa.EsPadre)
                    .Select(aa => new ApoderadoInlineViewModel
                    {
                        ApoderadoID = aa.ApoderadoID,
                        Rut = RutHelper.Formatear(aa.tbl_Apoderado.Rut),
                        Nombres = aa.tbl_Apoderado.Nombres,
                        ApellidoPaterno = aa.tbl_Apoderado.ApellidoPaterno,
                        ApellidoMaterno = aa.tbl_Apoderado.ApellidoMaterno,
                        NacionalidadID = aa.tbl_Apoderado.NacionalidadID,
                        NivelEducacionalID = aa.tbl_Apoderado.NivelEducacionalID,
                        SituacionLaboralID = aa.tbl_Apoderado.SituacionLaboralID,
                        LugarTrabajo = aa.tbl_Apoderado.LugarTrabajo,
                        Direccion = aa.tbl_Apoderado.Direccion,
                        RegionID = aa.tbl_Apoderado.tbl_Comuna != null ? aa.tbl_Apoderado.tbl_Comuna.RegionID : (int?)null,
                        ComunaID = aa.tbl_Apoderado.ComunaID,
                        Telefono = aa.tbl_Apoderado.Telefono,
                        TelefonoCelular = aa.tbl_Apoderado.TelefonoCelular,
                        Email = aa.tbl_Apoderado.Email,
                        ParentescoID = aa.ParentescoID,
                        EsApoderadoTitular = aa.EsApoderadoTitular
                    })
                    .FirstOrDefault() ?? new ApoderadoInlineViewModel(),
                ApoderadoMadre = alumno.tbl_AlumnoApoderado
                    .Where(aa => aa.IsActive && aa.EsMadre)
                    .Select(aa => new ApoderadoInlineViewModel
                    {
                        ApoderadoID = aa.ApoderadoID,
                        Rut = RutHelper.Formatear(aa.tbl_Apoderado.Rut),
                        Nombres = aa.tbl_Apoderado.Nombres,
                        ApellidoPaterno = aa.tbl_Apoderado.ApellidoPaterno,
                        ApellidoMaterno = aa.tbl_Apoderado.ApellidoMaterno,
                        NacionalidadID = aa.tbl_Apoderado.NacionalidadID,
                        NivelEducacionalID = aa.tbl_Apoderado.NivelEducacionalID,
                        SituacionLaboralID = aa.tbl_Apoderado.SituacionLaboralID,
                        LugarTrabajo = aa.tbl_Apoderado.LugarTrabajo,
                        Direccion = aa.tbl_Apoderado.Direccion,
                        RegionID = aa.tbl_Apoderado.tbl_Comuna != null ? aa.tbl_Apoderado.tbl_Comuna.RegionID : (int?)null,
                        ComunaID = aa.tbl_Apoderado.ComunaID,
                        Telefono = aa.tbl_Apoderado.Telefono,
                        TelefonoCelular = aa.tbl_Apoderado.TelefonoCelular,
                        Email = aa.tbl_Apoderado.Email,
                        ParentescoID = aa.ParentescoID,
                        EsApoderadoTitular = aa.EsApoderadoTitular
                    })
                    .FirstOrDefault() ?? new ApoderadoInlineViewModel(),
                // Foto servida desde BD via endpoint /Documento/FotoAlumno/{id}
                FotoUrl = alumno.FotoContenido != null ? $"/Documento/FotoAlumno/{alumno.ID}" : null,
                MatriculaID = matriculaVigente?.ID,
                CursoID = matriculaVigente?.CursoID,
                Curso = matriculaVigente != null
                    ? $"{matriculaVigente.tbl_Curso.tbl_Grado.Nombre} {matriculaVigente.tbl_Curso.Letra}"
                    : null,
                AnioEscolar = matriculaVigente?.tbl_AnioEscolar.Anio,
                EsAlumnoNuevo = matriculaVigente?.EsAlumnoNuevo ?? false,
                ResultadoPromocion = matriculaVigente?.ResultadoPromocion,
                MatriculaCancelada = matriculaVigente?.MatriculaCancelada ?? false,
                MotivoNoPromocion = matriculaVigente?.MotivoNoPromocion,
                DecretoNoPromocion = matriculaVigente?.DecretoNoPromocion,
                GlosaNoPromocion = matriculaVigente?.GlosaNoPromocion,
                ProfesorJefe = matriculaVigente?.tbl_Curso?.ProfesorJefeID != null
                    ? _dbContext.tbl_ProfesorJefe
                        .Where(p => p.ID == matriculaVigente.tbl_Curso.ProfesorJefeID)
                        .Select(p => p.Nombre + " " + p.Apellido)
                        .FirstOrDefault()
                    : null,
                EmailProfesorJefe = matriculaVigente?.tbl_Curso?.ProfesorJefeID != null
                    ? _dbContext.tbl_ProfesorJefe
                        .Where(p => p.ID == matriculaVigente.tbl_Curso.ProfesorJefeID)
                        .Select(p => p.Email)
                        .FirstOrDefault()
                    : null,
                Apoderados = alumno.tbl_AlumnoApoderado
                    .Where(aa => aa.IsActive && aa.tbl_Apoderado != null)
                    .Select(aa => new ApoderadoResumenViewModel
                    {
                        ID = aa.tbl_Apoderado.ID,
                        AlumnoApoderadoID = aa.ID,
                        Rut = RutHelper.Formatear(aa.tbl_Apoderado.Rut),
                        NombreCompleto = aa.tbl_Apoderado.ApellidoPaterno + " " + aa.tbl_Apoderado.ApellidoMaterno + ", " + aa.tbl_Apoderado.Nombres,
                        Parentesco = aa.tbl_Parentesco != null ? aa.tbl_Parentesco.Nombre : null,
                        EsApoderadoTitular = aa.EsApoderadoTitular,
                        EsPadre = aa.EsPadre,
                        EsMadre = aa.EsMadre,
                        Telefono = aa.tbl_Apoderado.Telefono,
                        TelefonoCelular = aa.tbl_Apoderado.TelefonoCelular,
                        Email = aa.tbl_Apoderado.Email,
                        NivelEducacional = aa.tbl_Apoderado.tbl_NivelEducacional != null ? aa.tbl_Apoderado.tbl_NivelEducacional.Nombre : null,
                        SituacionLaboral = aa.tbl_Apoderado.tbl_SituacionLaboral != null ? aa.tbl_Apoderado.tbl_SituacionLaboral.Nombre : null,
                        LugarTrabajo = aa.tbl_Apoderado.LugarTrabajo
                    }).ToList(),
                Documentos = alumno.tbl_DocumentoAlumno
                    .Where(d => d.IsActive)
                    .Select(d => new DocumentoAlumnoViewModel
                    {
                        ID = d.ID,
                        TipoDocumento = d.tbl_TipoDocumento.Nombre,
                        NombreArchivo = d.NombreArchivo,
                        FechaCarga = d.FechaCarga,
                        Obligatorio = d.tbl_TipoDocumento.Obligatorio
                    }).ToList(),
                Historial = await GetHistorialAsync(alumnoId)
            };
        }

        public async Task<int> CreateAlumnoAsync(AlumnoFichaViewModel model, string createdBy)
        {
            if (await RutExisteAsync(model.Rut))
                throw new InvalidOperationException("El RUT ingresado ya se encuentra registrado.");

            var alumno = new tbl_Alumno
            {
                Rut = model.Rut?.Replace(".", "").Trim().ToUpper(),
                Nombres = model.Nombres,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno,
                FechaNacimiento = model.FechaNacimiento,
                SexoID = model.SexoID,
                NacionalidadID = model.NacionalidadID,
                Direccion = model.Direccion,
                ComunaID = model.ComunaID,
                Telefono = model.Telefono,
                Email = model.Email,
                SistemaSaludID = model.SistemaSaludID,
                TienePIE = model.TienePIE,
                EtniaID = model.EtniaID,
                CondicionSocioeconomicaID = model.CondicionSocioeconomicaID,
                ViveConID = model.ViveConID,
                EstadoAlumnoID = model.EstadoAlumnoID > 0 ? model.EstadoAlumnoID : 1,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            _dbContext.tbl_Alumno.Add(alumno);
            await _dbContext.SaveChangesAsync();

            if (model.AlergiasPendientes != null && model.AlergiasPendientes.Count > 0)
                await PersistirAlergiasPendientesAsync(alumno.ID, model.AlergiasPendientes, createdBy);

            if (model.DiscapacidadesPendientes != null && model.DiscapacidadesPendientes.Count > 0)
                await PersistirDiscapacidadesPendientesAsync(alumno.ID, model.DiscapacidadesPendientes, createdBy);

            await GuardarApoderadoInlineAsync(alumno.ID, model.ApoderadoPadre, esPadre: true, esMadre: false, createdBy);
            await GuardarApoderadoInlineAsync(alumno.ID, model.ApoderadoMadre, esPadre: false, esMadre: true, createdBy);

            await RegistrarLogAsync("Alumno", alumno.ID, "REGISTRÓ AL ALUMNO",
                createdBy, $"RUT: {alumno.Rut}");
            return alumno.ID;
        }

        public async Task<int> UpdateAlumnoAsync(AlumnoFichaViewModel model, string modifiedBy)
        {
            if (await RutExisteAsync(model.Rut, model.ID))
                throw new InvalidOperationException("El RUT ingresado ya se encuentra registrado por otro alumno.");

            var alumno = await _dbContext.tbl_Alumno.FindAsync(model.ID);
            if (alumno == null) return 0;

            alumno.Nombres = model.Nombres;
            alumno.ApellidoPaterno = model.ApellidoPaterno;
            alumno.ApellidoMaterno = model.ApellidoMaterno;
            alumno.FechaNacimiento = model.FechaNacimiento;
            alumno.SexoID = model.SexoID;
            alumno.NacionalidadID = model.NacionalidadID;
            alumno.Direccion = model.Direccion;
            alumno.ComunaID = model.ComunaID;
            alumno.Telefono = model.Telefono;
            alumno.Email = model.Email;
            alumno.SistemaSaludID = model.SistemaSaludID;
            alumno.TienePIE = model.TienePIE;
            alumno.EtniaID = model.EtniaID;
            alumno.CondicionSocioeconomicaID = model.CondicionSocioeconomicaID;
            alumno.ViveConID = model.ViveConID;
            alumno.EstadoAlumnoID = model.EstadoAlumnoID;
            alumno.ModifiedDate = DateTime.UtcNow;
            alumno.ModifiedBy = modifiedBy;

            await _dbContext.SaveChangesAsync();

            if (model.AlergiasPendientes != null && model.AlergiasPendientes.Count > 0)
                await PersistirAlergiasPendientesAsync(alumno.ID, model.AlergiasPendientes, modifiedBy);

            if (model.DiscapacidadesPendientes != null && model.DiscapacidadesPendientes.Count > 0)
                await PersistirDiscapacidadesPendientesAsync(alumno.ID, model.DiscapacidadesPendientes, modifiedBy);

            await GuardarApoderadoInlineAsync(alumno.ID, model.ApoderadoPadre, esPadre: true, esMadre: false, modifiedBy);
            await GuardarApoderadoInlineAsync(alumno.ID, model.ApoderadoMadre, esPadre: false, esMadre: true, modifiedBy);

            await RegistrarLogAsync("Alumno", alumno.ID, "EDITÓ LA FICHA DEL ALUMNO",
                modifiedBy, $"RUT: {alumno.Rut}");
            return alumno.ID;
        }

        private async Task GuardarApoderadoInlineAsync(int alumnoId, ApoderadoInlineViewModel vm,
            bool esPadre, bool esMadre, string createdBy)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.Rut)) return;

            var rutLimpio = vm.Rut.Replace(".", "").Replace("-", "").Trim().ToUpper();

            // Buscar o crear el apoderado
            tbl_Apoderado entidad;
            if (vm.ApoderadoID > 0)
            {
                entidad = await _dbContext.tbl_Apoderado.FindAsync(vm.ApoderadoID);
                if (entidad == null) return;
            }
            else
            {
                entidad = await _dbContext.tbl_Apoderado
                    .FirstOrDefaultAsync(x => x.Rut == rutLimpio && x.IsActive);

                if (entidad == null)
                {
                    entidad = new tbl_Apoderado { IsActive = true, CreatedDate = DateTime.UtcNow, CreatedBy = createdBy };
                    _dbContext.tbl_Apoderado.Add(entidad);
                }
            }

            entidad.Rut = rutLimpio;
            entidad.Nombres = vm.Nombres;
            entidad.ApellidoPaterno = vm.ApellidoPaterno;
            entidad.ApellidoMaterno = vm.ApellidoMaterno;
            entidad.NacionalidadID = vm.NacionalidadID;
            entidad.NivelEducacionalID = vm.NivelEducacionalID;
            entidad.SituacionLaboralID = vm.SituacionLaboralID;
            entidad.LugarTrabajo = vm.LugarTrabajo;
            entidad.Direccion = vm.Direccion;
            entidad.ComunaID = vm.ComunaID;
            entidad.Telefono = vm.Telefono;
            entidad.TelefonoCelular = vm.TelefonoCelular;
            entidad.Email = vm.Email;

            await _dbContext.SaveChangesAsync();

            // Crear o actualizar el vínculo alumno–apoderado
            var vinculo = await _dbContext.tbl_AlumnoApoderado
                .FirstOrDefaultAsync(aa => aa.AlumnoID == alumnoId
                                        && aa.ApoderadoID == entidad.ID
                                        && aa.IsActive);

            if (vinculo == null)
            {
                if (vm.EsApoderadoTitular)
                {
                    // Quitar titular actual
                    var titularActual = await _dbContext.tbl_AlumnoApoderado
                        .FirstOrDefaultAsync(aa => aa.AlumnoID == alumnoId && aa.EsApoderadoTitular && aa.IsActive);
                    if (titularActual != null) titularActual.EsApoderadoTitular = false;
                }
                vinculo = new tbl_AlumnoApoderado
                {
                    AlumnoID = alumnoId,
                    ApoderadoID = entidad.ID,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                _dbContext.tbl_AlumnoApoderado.Add(vinculo);
            }

            vinculo.ParentescoID = vm.ParentescoID;
            vinculo.EsApoderadoTitular = vm.EsApoderadoTitular;
            vinculo.EsPadre = esPadre;
            vinculo.EsMadre = esMadre;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> RetirarAlumnoAsync(int alumnoId, int causalRetiroId, DateTime fechaRetiro, string observacion, string createdBy)
        {
            var alumno = await _dbContext.tbl_Alumno.FindAsync(alumnoId);
            if (alumno == null) return 0;

            // Cambiar estado a Retirado (ID=2)
            alumno.EstadoAlumnoID = 2;
            alumno.ModifiedDate = DateTime.UtcNow;
            alumno.ModifiedBy = createdBy;

            _dbContext.tbl_RetiroAlumno.Add(new tbl_RetiroAlumno
            {
                AlumnoID = alumnoId,
                CausalRetiroID = causalRetiroId,
                FechaRetiro = fechaRetiro,
                Observacion = observacion,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            });

            await _dbContext.SaveChangesAsync();
            var causal = await _dbContext.tbl_CausalRetiro.FindAsync(causalRetiroId);
            await RegistrarLogAsync("Alumno", alumnoId, "RETIRÓ AL ALUMNO",
                createdBy,
                $"Causal: {causal?.Nombre ?? causalRetiroId.ToString()} | Fecha retiro: {fechaRetiro:dd/MM/yyyy}" +
                (string.IsNullOrWhiteSpace(observacion) ? "" : $" | Obs: {observacion}"));
            return alumnoId;
        }

        public async Task<List<LogActividadViewModel>> GetHistorialAsync(int alumnoId)
        {
            return await _dbContext.tbl_LogActividad
                .Where(l => l.EntidadID == alumnoId)
                .OrderByDescending(l => l.FechaAccion)
                .Select(l => new LogActividadViewModel
                {
                    ID = l.ID,
                    Entidad = l.Entidad,
                    Usuario = l.Usuario,
                    Accion = l.Accion,
                    Detalle = l.Detalle,
                    FechaAccion = l.FechaAccion
                })
                .ToListAsync();
        }

        public async Task<bool> RutExisteAsync(string rut, int excludeAlumnoId = 0)
        {
            if (string.IsNullOrWhiteSpace(rut)) return false;
            var rutNorm = rut.Replace(".", "").Trim().ToUpper();
            return await _dbContext.tbl_Alumno
                .AnyAsync(a => a.IsActive
                            && a.Rut.ToUpper() == rutNorm
                            && a.ID != excludeAlumnoId);
        }

        public async Task<List<AlumnoAlergiaViewModel>> GetAlergiasAlumnoAsync(int alumnoId)
        {
            return await _dbContext.tbl_AlumnoAlergia
                .Include(al => al.tbl_TipoAlergia)
                .Where(al => al.AlumnoID == alumnoId && al.IsActive)
                .Select(al => new AlumnoAlergiaViewModel
                {
                    ID = al.ID,
                    AlumnoID = al.AlumnoID,
                    TipoAlergiaID = al.TipoAlergiaID,
                    TipoAlergia = al.tbl_TipoAlergia.Nombre,
                    NombreAlergia = al.NombreAlergia,
                    Descripcion = al.Descripcion,
                    CertificadoNombre = al.CertificadoNombre,
                    CertificadoMimeType = al.CertificadoMimeType
                })
                .ToListAsync();
        }

        /// <summary>
        /// Persiste la lista de alergias pendientes enviadas desde el formulario.
        /// Llamado desde CreateAlumnoAsync y UpdateAlumnoAsync.
        /// </summary>
        private async Task PersistirAlergiasPendientesAsync(
            int alumnoId,
            List<AlumnoAlergiaViewModel> pendientes,
            string usuario)
        {
            foreach (var vm in pendientes)
            {
                if (vm.TipoAlergiaID == 0) continue; // fila vacía ignorada

                var entidad = new tbl_AlumnoAlergia
                {
                    AlumnoID = alumnoId,
                    TipoAlergiaID = vm.TipoAlergiaID,
                    NombreAlergia = vm.NombreAlergia,
                    Descripcion = vm.Descripcion,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = usuario
                };

                if (vm.CertificadoArchivo != null && vm.CertificadoArchivo.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        vm.CertificadoArchivo.InputStream.CopyTo(ms);
                        entidad.CertificadoContenido = ms.ToArray();
                    }
                    entidad.CertificadoNombre = Path.GetFileName(vm.CertificadoArchivo.FileName);
                    entidad.CertificadoMimeType = vm.CertificadoArchivo.ContentType;
                }

                _dbContext.tbl_AlumnoAlergia.Add(entidad);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GuardarAlergiaAsync(AlumnoAlergiaViewModel model, string createdBy)
        {
            tbl_AlumnoAlergia entidad;
            if (model.ID > 0)
            {
                entidad = await _dbContext.tbl_AlumnoAlergia.FindAsync(model.ID);
                if (entidad == null) return 0;
                entidad.TipoAlergiaID = model.TipoAlergiaID;
                entidad.NombreAlergia = model.NombreAlergia;
                entidad.Descripcion = model.Descripcion;
                entidad.ModifiedDate = DateTime.UtcNow;
                entidad.ModifiedBy = createdBy;
            }
            else
            {
                entidad = new tbl_AlumnoAlergia
                {
                    AlumnoID = model.AlumnoID,
                    TipoAlergiaID = model.TipoAlergiaID,
                    NombreAlergia = model.NombreAlergia,
                    Descripcion = model.Descripcion,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                _dbContext.tbl_AlumnoAlergia.Add(entidad);
            }

            if (model.CertificadoArchivo != null && model.CertificadoArchivo.ContentLength > 0)
            {
                var buffer = new byte[model.CertificadoArchivo.ContentLength];
                model.CertificadoArchivo.InputStream.Read(buffer, 0, buffer.Length);
                entidad.CertificadoContenido = buffer;
                entidad.CertificadoNombre = System.IO.Path.GetFileName(model.CertificadoArchivo.FileName);
                entidad.CertificadoMimeType = model.CertificadoArchivo.ContentType;
            }

            await _dbContext.SaveChangesAsync();
            var tipoNombre = model.TipoAlergiaID == 1 ? "Informativa" : "Alimenticia";
            await RegistrarLogAsync("Alergia", model.AlumnoID,
                model.ID > 0 ? "EDITÓ ALERGIA" : "REGISTRÓ ALERGIA",
                createdBy,
                $"Tipo: {tipoNombre} | {model.NombreAlergia}{(string.IsNullOrWhiteSpace(model.Descripcion) ? "" : ": " + model.Descripcion)}");
            return entidad.ID;
        }

        public async Task<int> EliminarAlergiaAsync(int alergiaId, string modifiedBy)
        {
            var entidad = await _dbContext.tbl_AlumnoAlergia.FindAsync(alergiaId);
            if (entidad == null) return 0;
            entidad.IsActive = false;
            entidad.ModifiedDate = DateTime.UtcNow;
            entidad.ModifiedBy = modifiedBy;
            await _dbContext.SaveChangesAsync();
            await RegistrarLogAsync("Alergia", entidad.AlumnoID, "ELIMINÓ ALERGIA",
                modifiedBy, entidad.NombreAlergia);
            return alergiaId;
        }

        public async Task<(byte[] Contenido, string Nombre, string MimeType)> DescargarCertificadoAlergiaAsync(int alergiaId)
        {
            var entidad = await _dbContext.tbl_AlumnoAlergia.FindAsync(alergiaId);
            if (entidad == null || entidad.CertificadoContenido == null)
                return (null, null, null);
            return (entidad.CertificadoContenido, entidad.CertificadoNombre, entidad.CertificadoMimeType);
        }

        // ────────────────────────────────────────────────────────────────
        // DISCAPACIDADES
        // ────────────────────────────────────────────────────────────────
        private async Task PersistirDiscapacidadesPendientesAsync(
            int alumnoId,
            List<AlumnoDiscapacidadViewModel> pendientes,
            string createdBy)
        {
            foreach (var vm in pendientes)
            {
                if (vm.TipoDiscapacidadID == 0) continue;
                if (vm.CertificadoArchivo == null || vm.CertificadoArchivo.ContentLength == 0)
                    continue; // certificado obligatorio — no persiste sin él

                byte[] contenido;
                using (var ms = new System.IO.MemoryStream())
                {
                    vm.CertificadoArchivo.InputStream.CopyTo(ms);
                    contenido = ms.ToArray();
                }

                _dbContext.tbl_AlumnoDiscapacidad.Add(new tbl_AlumnoDiscapacidad
                {
                    AlumnoID = alumnoId,
                    TipoDiscapacidadID = vm.TipoDiscapacidadID,
                    Descripcion = vm.Descripcion,
                    CertificadoContenido = contenido,
                    CertificadoNombre = System.IO.Path.GetFileName(vm.CertificadoArchivo.FileName),
                    CertificadoMimeType = vm.CertificadoArchivo.ContentType,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AlumnoDiscapacidadViewModel>> GetDiscapacidadesAlumnoAsync(int alumnoId)
        {
            return await _dbContext.tbl_AlumnoDiscapacidad
                .Include(d => d.tbl_TipoDiscapacidad)
                .Where(d => d.AlumnoID == alumnoId && d.IsActive)
                .Select(d => new AlumnoDiscapacidadViewModel
                {
                    ID = d.ID,
                    AlumnoID = d.AlumnoID,
                    TipoDiscapacidadID = d.TipoDiscapacidadID,
                    TipoDiscapacidad = d.tbl_TipoDiscapacidad.Nombre,
                    Descripcion = d.Descripcion,
                    CertificadoNombre = d.CertificadoNombre,
                    CertificadoMimeType = d.CertificadoMimeType
                })
                .ToListAsync();
        }

        public async Task<int> EliminarDiscapacidadAsync(int discapacidadId, string modifiedBy)
        {
            var entidad = await _dbContext.tbl_AlumnoDiscapacidad.FindAsync(discapacidadId);
            if (entidad == null) return 0;
            entidad.IsActive = false;
            entidad.ModifiedDate = DateTime.UtcNow;
            entidad.ModifiedBy = modifiedBy;
            await _dbContext.SaveChangesAsync();
            await RegistrarLogAsync("Alumno", entidad.AlumnoID, "ELIMINÓ DISCAPACIDAD",
                modifiedBy, entidad.tbl_TipoDiscapacidad?.Nombre ?? discapacidadId.ToString());
            return discapacidadId;
        }

        public async Task<(byte[] Contenido, string Nombre, string MimeType)>
            DescargarCertificadoDiscapacidadAsync(int discapacidadId)
        {
            var entidad = await _dbContext.tbl_AlumnoDiscapacidad.FindAsync(discapacidadId);
            if (entidad == null || entidad.CertificadoContenido == null)
                return (null, null, null);
            return (entidad.CertificadoContenido, entidad.CertificadoNombre, entidad.CertificadoMimeType);
        }

        // ────────────────────────────────────────────────────────────────
        // CAMBIO DE RUT (extranjeros que obtienen RUT chileno)
        // ────────────────────────────────────────────────────────────────
        public async Task<(int NuevoAlumnoId, string Error)> CambioRutAsync(
            CambioRutViewModel model, string createdBy)
        {
            // 1. Validar que el RUT nuevo no esté en uso
            if (await RutExisteAsync(model.RutNuevo))
                return (0, "El RUT ingresado ya está registrado en el sistema.");

            // 2. Cargar alumno origen completo con todas sus relaciones
            var origen = await _dbContext.tbl_Alumno
                .Include(a => a.tbl_AlumnoApoderado)
                .Include(a => a.tbl_AlumnoAlergia)
                .Include(a => a.tbl_AlumnoDiscapacidad)
                .Include(a => a.tbl_DocumentoAlumno)
                .Include(a => a.tbl_Matricula)
                .FirstOrDefaultAsync(a => a.ID == model.AlumnoOrigenID && a.IsActive);

            if (origen == null)
                return (0, "No se encontró el alumno de origen.");

            // 3. Crear nuevo alumno con RUT definitivo (copia exacta de todos los datos)
            var destino = new tbl_Alumno
            {
                Rut = model.RutNuevo,
                RutAnterior = origen.Rut,
                AlumnoOrigenID = origen.ID,
                Nombres = origen.Nombres,
                ApellidoPaterno = origen.ApellidoPaterno,
                ApellidoMaterno = origen.ApellidoMaterno,
                FechaNacimiento = origen.FechaNacimiento,
                SexoID = origen.SexoID,
                NacionalidadID = origen.NacionalidadID,
                Direccion = origen.Direccion,
                ComunaID = origen.ComunaID,
                Telefono = origen.Telefono,
                Email = origen.Email,
                SistemaSaludID = origen.SistemaSaludID,
                TienePIE = origen.TienePIE,
                EtniaID = origen.EtniaID,
                CondicionSocioeconomicaID = origen.CondicionSocioeconomicaID,
                ViveConID = origen.ViveConID,
                EstadoAlumnoID = origen.EstadoAlumnoID,
                FotoContenido = origen.FotoContenido,
                FotoMimeType = origen.FotoMimeType,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            _dbContext.tbl_Alumno.Add(destino);
            await _dbContext.SaveChangesAsync(); // necesitamos el ID del destino

            // 4. COPIAR matrículas activas al nuevo alumno (los originales quedan en el origen)
            foreach (var mat in origen.tbl_Matricula.Where(m => m.IsActive))
            {
                _dbContext.tbl_Matricula.Add(new tbl_Matricula
                {
                    AlumnoID = destino.ID,
                    AnioEscolarID = mat.AnioEscolarID,
                    CursoID = mat.CursoID,
                    NroMatricula = mat.NroMatricula,
                    FechaMatricula = mat.FechaMatricula,
                    EstadoMatriculaID = mat.EstadoMatriculaID,
                    EsAlumnoNuevo = mat.EsAlumnoNuevo,
                    Observacion = mat.Observacion,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }

            // 5. COPIAR apoderados activos al nuevo alumno
            foreach (var aa in origen.tbl_AlumnoApoderado.Where(a => a.IsActive))
            {
                _dbContext.tbl_AlumnoApoderado.Add(new tbl_AlumnoApoderado
                {
                    AlumnoID = destino.ID,
                    ApoderadoID = aa.ApoderadoID,
                    ParentescoID = aa.ParentescoID,
                    EsApoderadoTitular = aa.EsApoderadoTitular,
                    EsPadre = aa.EsPadre,
                    EsMadre = aa.EsMadre,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }

            // 6. COPIAR alergias activas al nuevo alumno
            foreach (var al in origen.tbl_AlumnoAlergia.Where(a => a.IsActive))
            {
                _dbContext.tbl_AlumnoAlergia.Add(new tbl_AlumnoAlergia
                {
                    AlumnoID = destino.ID,
                    TipoAlergiaID = al.TipoAlergiaID,
                    NombreAlergia = al.NombreAlergia,
                    Descripcion = al.Descripcion,
                    CertificadoNombre = al.CertificadoNombre,
                    CertificadoMimeType = al.CertificadoMimeType,
                    CertificadoContenido = al.CertificadoContenido,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }

            // 7. COPIAR discapacidades activas al nuevo alumno
            foreach (var disc in origen.tbl_AlumnoDiscapacidad.Where(d => d.IsActive))
            {
                _dbContext.tbl_AlumnoDiscapacidad.Add(new tbl_AlumnoDiscapacidad
                {
                    AlumnoID = destino.ID,
                    TipoDiscapacidadID = disc.TipoDiscapacidadID,
                    Descripcion = disc.Descripcion,
                    CertificadoNombre = disc.CertificadoNombre,
                    CertificadoMimeType = disc.CertificadoMimeType,
                    CertificadoContenido = disc.CertificadoContenido,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }

            // 8. COPIAR documentos activos al nuevo alumno
            foreach (var doc in origen.tbl_DocumentoAlumno.Where(d => d.IsActive))
            {
                _dbContext.tbl_DocumentoAlumno.Add(new tbl_DocumentoAlumno
                {
                    AlumnoID = destino.ID,
                    AnioEscolarID = doc.AnioEscolarID,
                    TipoDocumentoID = doc.TipoDocumentoID,
                    NombreArchivo = doc.NombreArchivo,
                    MimeType = doc.MimeType,
                    Contenido = doc.Contenido,
                    Observacion = doc.Observacion,
                    FechaCarga = doc.FechaCarga,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                });
            }

            // 9. Marcar el alumno origen como inactivo/traspasado (su ficha queda completa como historial)
            origen.IsActive = false;
            origen.EstadoAlumnoID = 5; // "Traspasado por cambio de RUT"
            origen.ModifiedDate = DateTime.UtcNow;
            origen.ModifiedBy = createdBy;

            // 10. Registrar el traspaso
            _dbContext.tbl_TraspasoRut.Add(new tbl_TraspasoRut
            {
                AlumnoOrigenID = origen.ID,
                AlumnoDestinoID = destino.ID,
                RutAnterior = origen.Rut,
                RutNuevo = model.RutNuevo,
                FechaTraspaso = DateTime.Today,
                Motivo = model.Motivo,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            });

            await _dbContext.SaveChangesAsync();

            // 11. Registrar logs
            await RegistrarLogAsync("Alumno", destino.ID, "CAMBIO DE RUT (EXTRANJERO)",
                createdBy,
                $"RUT anterior: {origen.Rut} → RUT nuevo: {model.RutNuevo}" +
                (string.IsNullOrWhiteSpace(model.Motivo) ? "" : $" | Motivo: {model.Motivo}"));

            await RegistrarLogAsync("Alumno", origen.ID, "ALUMNO TRASPASADO POR CAMBIO DE RUT",
                createdBy,
                $"Nuevo registro creado con RUT: {model.RutNuevo} (AlumnoID: {destino.ID})");

            return (destino.ID, null);
        }

        public async Task<TraspasoRutResumenViewModel> GetTraspasoRutAsync(int alumnoId)
        {
            return await _dbContext.tbl_TraspasoRut
                .Where(t => t.AlumnoOrigenID == alumnoId || t.AlumnoDestinoID == alumnoId)
                .OrderByDescending(t => t.FechaTraspaso)
                .Select(t => new TraspasoRutResumenViewModel
                {
                    ID = t.ID,
                    RutAnterior = t.RutAnterior,
                    RutNuevo = t.RutNuevo,
                    FechaTraspaso = t.FechaTraspaso,
                    Motivo = t.Motivo,
                    AlumnoOrigenID = t.AlumnoOrigenID,
                    AlumnoDestinoID = t.AlumnoDestinoID
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<SelectItemViewModel>> GetEstadosAlumnoAsync()
        {
            return await _dbContext.tbl_EstadoAlumno
                .Where(e => e.IsActive)
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToListAsync();
        }

        public async Task<List<int>> GetAniosEscolaresAlumnosAsync()
        {
            return await _dbContext.tbl_Matricula
                .Where(m => m.IsActive && m.tbl_AnioEscolar != null)
                .Select(m => m.tbl_AnioEscolar.Anio)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();
        }

        public async Task<int> GetAnioActivoAsync()
        {
            var anio = await GetAnioEscolarActivoAsync();
            return anio?.Anio ?? 0;
        }
    }
}
