using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using GestionColegios.Helpers;
using KendoNET.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class AnioEscolarService : BaseServices, IAnioEscolarService
    {
        private readonly IAuthenticationService _authenticationService;

        public AnioEscolarService(
            Entities dbContext,
            IMapperService mapperService,
            IAuthenticationService authenticationService)
            : base(dbContext, mapperService)
        {
            _authenticationService = authenticationService;
        }

        // ?? Años escolares ????????????????????????????????????????????????

        public async Task<DataSourceResult> GetAniosEscolaresAsync(DataSourceRequest request)
        {
            var query = _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive)
                .Select(a => new AnioEscolarListadoViewModel
                {
                    ID              = a.ID,
                    Anio            = a.Anio,
                    Establecimiento = a.tbl_Establecimiento.Nombre,
                    FechaInicio     = a.FechaInicio,
                    FechaTermino    = a.FechaTermino,
                    Cerrado         = a.Cerrado,
                    EsActivo        = a.EsActivo,
                    TotalCursos     = a.tbl_Curso.Count(c => c.IsActive),
                    TotalMatriculados = _dbContext.tbl_Matricula
                        .Count(m => m.IsActive && m.AnioEscolarID == a.ID)
                });

            return await query
                .OrderByDescending(a => a.Anio)
                .ToDataSourceResultAsync(
                    request.Take, request.Skip,
                    request.Sort, request.Filter,
                    request.Aggregate, request.Group);
        }

        public async Task<AnioEscolarDetalleViewModel> GetDetalleAsync(int id)
        {
            var anio = await _dbContext.tbl_AnioEscolar
                .Include(a => a.tbl_Establecimiento)
                .FirstOrDefaultAsync(a => a.ID == id && a.IsActive);

            if (anio == null) return null;

            var cursos = await (
                from c in _dbContext.tbl_Curso
                join g in _dbContext.tbl_Grado on c.GradoID equals g.ID
                join n in _dbContext.tbl_NivelEnsenanza on g.NivelEnsenanzaID equals n.ID
                join pj in _dbContext.tbl_ProfesorJefe on c.ProfesorJefeID equals pj.ID into profJoin
                from prof in profJoin.DefaultIfEmpty()
                where c.AnioEscolarID == id && c.IsActive
                orderby g.Orden, c.Letra
                select new CursoDetalleViewModel
                {
                    ID             = c.ID,
                    AnioEscolarID  = c.AnioEscolarID,
                    GradoID        = c.GradoID,
                    NivelEnsenanza = n.Nombre,
                    Grado          = g.Nombre,
                    Letra          = c.Letra,
                    Capacidad      = c.Capacidad,
                    ProfesorJefeID = c.ProfesorJefeID,
                    ProfesorJefe   = prof != null ? prof.Nombre + " " + prof.Apellido : null,
                    TotalAlumnos   = c.tbl_Matricula.Count(m => m.IsActive),
                    IsActive       = c.IsActive
                }
            ).ToListAsync();

            return new AnioEscolarDetalleViewModel
            {
                ID              = anio.ID,
                Anio            = anio.Anio,
                Establecimiento = anio.tbl_Establecimiento?.Nombre,
                FechaInicio     = anio.FechaInicio,
                FechaTermino    = anio.FechaTermino,
                Cerrado         = anio.Cerrado,
                Cursos          = cursos
            };
        }

        public async Task<AnioEscolarFormViewModel> GetFormAnioAsync(int id = 0)
        {
            var establecimientos = await _dbContext.tbl_Establecimiento
                .Where(e => e.IsActive)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToListAsync();

            if (id == 0)
                return new AnioEscolarFormViewModel
                {
                    Anio             = DateTime.Now.Year,
                    Establecimientos = establecimientos
                };

            var anio = await _dbContext.tbl_AnioEscolar.FindAsync(id);
            if (anio == null) return null;

            return new AnioEscolarFormViewModel
            {
                ID               = anio.ID,
                Anio             = anio.Anio,
                EstablecimientoID = anio.EstablecimientoID,
                FechaInicio      = anio.FechaInicio,
                FechaTermino     = anio.FechaTermino,
                Cerrado          = anio.Cerrado,
                EsActivo         = anio.EsActivo,
                Establecimientos = establecimientos
            };
        }

        public async Task<int> CreateAnioEscolarAsync(AnioEscolarFormViewModel model, string usuario)
        {
            // Verificar duplicado incluyendo registros inactivos (la constraint de BD no distingue IsActive)
            var existente = await _dbContext.tbl_AnioEscolar
                .FirstOrDefaultAsync(a => a.Anio == model.Anio
                                        && a.EstablecimientoID == model.EstablecimientoID);

            if (existente != null)
            {
                // Si existe pero está inactivo, se reactiva en lugar de crear uno nuevo
                if (!existente.IsActive)
                {
                    existente.IsActive = true;
                    existente.FechaInicio = model.FechaInicio;
                    existente.FechaTermino = model.FechaTermino;
                    existente.Cerrado = false;
                    existente.CreatedDate = DateTime.UtcNow;
                    existente.CreatedBy = usuario;
                    await _dbContext.SaveChangesAsync();
                    return existente.ID;
                }

                // Si está activo, es un duplicado real
                return -1;
            }

            var entidad = new tbl_AnioEscolar
            {
                Anio = model.Anio,
                EstablecimientoID = model.EstablecimientoID,
                FechaInicio = model.FechaInicio,
                FechaTermino = model.FechaTermino,
                Cerrado = false,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = usuario
            };
            _dbContext.tbl_AnioEscolar.Add(entidad);
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> UpdateAnioEscolarAsync(AnioEscolarFormViewModel model, string usuario)
        {
            var entidad = await _dbContext.tbl_AnioEscolar.FindAsync(model.ID);
            if (entidad == null) return 0;

            // Verificar duplicado excluyendo el propio registro
            var existe = await _dbContext.tbl_AnioEscolar
                .AnyAsync(a => a.Anio == model.Anio
                            && a.EstablecimientoID == model.EstablecimientoID
                            && a.IsActive
                            && a.ID != model.ID);
            if (existe) return -1;

            entidad.Anio              = model.Anio;
            entidad.EstablecimientoID = model.EstablecimientoID;
            entidad.FechaInicio       = model.FechaInicio;
            entidad.FechaTermino      = model.FechaTermino;
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> CerrarReobrirAnioAsync(int id, string usuario)
        {
            var entidad = await _dbContext.tbl_AnioEscolar.FindAsync(id);
            if (entidad == null) return 0;
            entidad.Cerrado = !entidad.Cerrado;
            // Si se cierra el año activo, quitar la marca EsActivo
            if (entidad.Cerrado && entidad.EsActivo)
                entidad.EsActivo = false;
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> MarcarComoActivoAsync(int id, string usuario)
        {
            var entidad = await _dbContext.tbl_AnioEscolar.FindAsync(id);
            if (entidad == null) return 0;
            if (entidad.Cerrado) return -1; // No se puede activar un año cerrado

            // Quitar EsActivo de todos los demás
            var todos = await _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive && a.EsActivo && a.ID != id)
                .ToListAsync();
            foreach (var a in todos)
                a.EsActivo = false;

            entidad.EsActivo = true;
            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Año Escolar", id, "MARCÓ AÑO COMO ACTIVO",
                usuario, $"Año: {entidad.Anio}");

            return entidad.ID;
        }

        // ?? Cierre de año: promoción de alumnos ???????????????????????????

        public async Task<DataSourceResult> GetPromocionAlumnosAsync(int anioEscolarId, DataSourceRequest request)
        {
            var lista = await _dbContext.tbl_Matricula
                .Where(m => m.IsActive
                         && m.AnioEscolarID == anioEscolarId
                         && m.tbl_EstadoMatricula.Nombre != "Anulada"
                         && m.tbl_EstadoMatricula.Nombre != "Lista de Espera")
                .OrderBy(m => m.tbl_Curso.tbl_Grado.Orden)
                .ThenBy(m => m.tbl_Curso.Letra)
                .ThenBy(m => m.tbl_Alumno.ApellidoPaterno)
                .Select(m => new PromocionAlumnoViewModel
                {
                    MatriculaID          = m.ID,
                    AlumnoID             = m.AlumnoID,
                    AlumnoNombreCompleto = m.tbl_Alumno.ApellidoPaterno + " " + m.tbl_Alumno.ApellidoMaterno + ", " + m.tbl_Alumno.Nombres,
                    AlumnoRut            = m.tbl_Alumno.Rut,
                    Curso                = m.tbl_Curso.tbl_Grado.Nombre + " " + m.tbl_Curso.Letra,
                    NroMatricula         = m.NroMatricula,
                    ResultadoPromocion   = m.ResultadoPromocion,
                    MatriculaCancelada   = m.MatriculaCancelada,
                    MotivoNoPromocion    = m.MotivoNoPromocion,
                    DecretoNoPromocion   = m.DecretoNoPromocion,
                    GlosaNoPromocion     = m.GlosaNoPromocion,
                    FechaResultadoPromocion = m.FechaResultadoPromocion
                })
                .ToListAsync();

            foreach (var item in lista)
                item.AlumnoRut = RutHelper.Formatear(item.AlumnoRut);

            return lista.AsQueryable().ToDataSourceResult(
                request.Take, request.Skip,
                request.Sort, request.Filter,
                request.Aggregate, request.Group);
        }

        public async Task<int> RegistrarPromocionAsync(RegistrarPromocionViewModel model, string usuario)
        {
            var matricula = await _dbContext.tbl_Matricula
                .Include(m => m.tbl_Alumno)
                .FirstOrDefaultAsync(m => m.ID == model.MatriculaID && m.IsActive);
            if (matricula == null) return 0;

            var esPromovido    = string.Equals(model.ResultadoPromocion, "Promovido", StringComparison.OrdinalIgnoreCase);
            var esNoPromovido  = string.Equals(model.ResultadoPromocion, "No Promovido", StringComparison.OrdinalIgnoreCase);

            if (!esPromovido && !esNoPromovido) return -1; // resultado inválido

            // No Promovido exige motivo, decreto y glosa
            if (esNoPromovido &&
                (string.IsNullOrWhiteSpace(model.MotivoNoPromocion)
                 || string.IsNullOrWhiteSpace(model.DecretoNoPromocion)
                 || string.IsNullOrWhiteSpace(model.GlosaNoPromocion)))
                return -2;

            matricula.ResultadoPromocion      = esPromovido ? "Promovido" : "No Promovido";
            matricula.FechaResultadoPromocion = DateTime.UtcNow;
            matricula.ModifiedDate            = DateTime.UtcNow;
            matricula.ModifiedBy              = usuario;

            if (esPromovido)
            {
                matricula.MatriculaCancelada = model.MatriculaCancelada;
                matricula.MotivoNoPromocion  = null;
                matricula.DecretoNoPromocion = null;
                matricula.GlosaNoPromocion   = null;
            }
            else // No Promovido
            {
                matricula.MatriculaCancelada = false;
                matricula.MotivoNoPromocion  = model.MotivoNoPromocion;
                matricula.DecretoNoPromocion = model.DecretoNoPromocion;
                matricula.GlosaNoPromocion   = model.GlosaNoPromocion;
            }

            // Reflejar el resultado en el estado del alumno
            var estadoAlumno = await _dbContext.tbl_EstadoAlumno
                .Where(e => e.IsActive && e.Nombre == matricula.ResultadoPromocion)
                .Select(e => e.ID)
                .FirstOrDefaultAsync();
            if (estadoAlumno > 0)
            {
                matricula.tbl_Alumno.EstadoAlumnoID = estadoAlumno;
                matricula.tbl_Alumno.ModifiedDate   = DateTime.UtcNow;
                matricula.tbl_Alumno.ModifiedBy     = usuario;
            }

            await _dbContext.SaveChangesAsync();

            var detalle = $"N°: {matricula.NroMatricula} | Resultado: {matricula.ResultadoPromocion}";
            if (esPromovido && model.MatriculaCancelada)
                detalle += " | MATRÍCULA CANCELADA (bloqueado para el año siguiente)";
            if (esNoPromovido)
                detalle += $" | Motivo: {model.MotivoNoPromocion} | Decreto: {model.DecretoNoPromocion}";

            await RegistrarLogAsync("Matrícula", matricula.AlumnoID, "REGISTRÓ RESULTADO DE PROMOCIÓN",
                usuario, detalle);

            return matricula.ID;
        }

        public async Task<int> PromoverCursoAsync(int anioEscolarId, int cursoId, string usuario)
        {
            // Matrículas activas del curso aún sin resultado de promoción (excluye anuladas y lista de espera)
            var matriculas = await _dbContext.tbl_Matricula
                .Include(m => m.tbl_Alumno)
                .Where(m => m.IsActive
                         && m.AnioEscolarID == anioEscolarId
                         && m.CursoID == cursoId
                         && m.ResultadoPromocion == null
                         && m.tbl_EstadoMatricula.Nombre != "Anulada"
                         && m.tbl_EstadoMatricula.Nombre != "Lista de Espera")
                .ToListAsync();

            if (matriculas.Count == 0) return 0;

            var estadoPromovido = await _dbContext.tbl_EstadoAlumno
                .Where(e => e.IsActive && e.Nombre == "Promovido")
                .Select(e => e.ID)
                .FirstOrDefaultAsync();

            var ahora = DateTime.UtcNow;
            foreach (var m in matriculas)
            {
                m.ResultadoPromocion      = "Promovido";
                m.MatriculaCancelada      = false;
                m.MotivoNoPromocion       = null;
                m.DecretoNoPromocion      = null;
                m.GlosaNoPromocion        = null;
                m.FechaResultadoPromocion = ahora;
                m.ModifiedDate            = ahora;
                m.ModifiedBy              = usuario;

                if (estadoPromovido > 0 && m.tbl_Alumno != null)
                {
                    m.tbl_Alumno.EstadoAlumnoID = estadoPromovido;
                    m.tbl_Alumno.ModifiedDate   = ahora;
                    m.tbl_Alumno.ModifiedBy     = usuario;
                }
            }

            await _dbContext.SaveChangesAsync();

            var curso = await _dbContext.tbl_Curso
                .Include(c => c.tbl_Grado)
                .FirstOrDefaultAsync(c => c.ID == cursoId);
            await RegistrarLogAsync("Año Escolar", anioEscolarId, "PROMOCIÓN MASIVA DE CURSO",
                usuario,
                $"Curso: {curso?.tbl_Grado?.Nombre} {curso?.Letra} | {matriculas.Count} alumno(s) marcados como Promovido.");

            return matriculas.Count;
        }

        public async Task<List<SelectItemViewModel>> GetCursosConPendientesAsync(int anioEscolarId)
        {
            return await _dbContext.tbl_Curso
                .Where(c => c.AnioEscolarID == anioEscolarId && c.IsActive
                         && c.tbl_Matricula.Any(m => m.IsActive
                                 && m.ResultadoPromocion == null
                                 && m.tbl_EstadoMatricula.Nombre != "Anulada"
                                 && m.tbl_EstadoMatricula.Nombre != "Lista de Espera"))
                .OrderBy(c => c.tbl_Grado.Orden)
                .ThenBy(c => c.Letra)
                .Select(c => new SelectItemViewModel
                {
                    ID    = c.ID,
                    Texto = c.tbl_Grado.Nombre + " " + c.Letra
                })
                .ToListAsync();
        }

        public async Task<(bool Ok, string Error)> AutorizarMatriculaCanceladaAsync(
            AutorizarMatriculaViewModel model, string usuario)
        {
            var matricula = await _dbContext.tbl_Matricula
                .FirstOrDefaultAsync(m => m.ID == model.MatriculaID && m.IsActive);
            if (matricula == null)
                return (false, "No se encontró la matrícula.");

            if (!matricula.MatriculaCancelada)
                return (false, "La matrícula no se encuentra en condición de cancelada.");

            // Validar la clave de supervisor: debe ser un usuario activo con rol Administrador
            var supervisor = await _dbContext.tbl_Usuarios
                .Include(u => u.tbl_RolesUsuarios.Select(ru => ru.tbl_Roles))
                .FirstOrDefaultAsync(u => u.Username == model.SupervisorUsuario && u.IsActive);

            if (supervisor == null)
                return (false, "Usuario supervisor no válido.");

            var esAdministrador = supervisor.IsSuperAdmin
                || supervisor.tbl_RolesUsuarios.Any(ru => ru.RolID == 1
                        || (ru.tbl_Roles != null && ru.tbl_Roles.Rol == "Administrador"));
            if (!esAdministrador)
                return (false, "El usuario indicado no tiene privilegios de supervisor.");

            if (!_authenticationService.VerifyPasswordHash(
                    model.SupervisorClave, supervisor.PasswordHash, supervisor.PasswordSalt))
                return (false, "Clave de supervisor incorrecta.");

            // Levantar el bloqueo
            matricula.MatriculaCancelada = false;
            matricula.ModifiedDate       = DateTime.UtcNow;
            matricula.ModifiedBy         = usuario;
            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Matrícula", matricula.AlumnoID, "AUTORIZÓ REINGRESO (LEVANTÓ MATRÍCULA CANCELADA)",
                usuario,
                $"N°: {matricula.NroMatricula} | Autorizado por supervisor: {supervisor.Username}" +
                (string.IsNullOrWhiteSpace(model.Observacion) ? "" : $" | Obs: {model.Observacion}"));

            return (true, null);
        }

        // ?? Cursos ????????????????????????????????????????????????????????

        public async Task<CursoFormViewModel> GetFormCursoAsync(int anioEscolarId, int cursoId = 0)
        {
            var grados    = await GetGradosAsync();
            var profesores = await GetProfesoresAsync();

            var anio = await _dbContext.tbl_AnioEscolar.FindAsync(anioEscolarId);

            if (cursoId == 0)
                return new CursoFormViewModel
                {
                    AnioEscolarID = anioEscolarId,
                    AnioEscolar   = anio?.Anio ?? 0,
                    Grados        = grados,
                    Profesores    = profesores
                };

            var curso = await _dbContext.tbl_Curso.FindAsync(cursoId);
            if (curso == null) return null;

            return new CursoFormViewModel
            {
                ID            = curso.ID,
                AnioEscolarID = curso.AnioEscolarID,
                AnioEscolar   = anio?.Anio ?? 0,
                GradoID       = curso.GradoID,
                Letra         = curso.Letra,
                Capacidad     = curso.Capacidad,
                ProfesorJefeID = curso.ProfesorJefeID,
                Grados        = grados,
                Profesores    = profesores
            };
        }

        public async Task<int> CreateCursoAsync(CursoFormViewModel model, string usuario)
        {
            // No duplicar grado + letra en el mismo año
            var existe = await _dbContext.tbl_Curso
                .AnyAsync(c => c.AnioEscolarID == model.AnioEscolarID
                            && c.GradoID == model.GradoID
                            && c.Letra.ToUpper() == model.Letra.ToUpper()
                            && c.IsActive);
            if (existe) return -1;

            var entidad = new tbl_Curso
            {
                AnioEscolarID  = model.AnioEscolarID,
                GradoID        = model.GradoID,
                Letra          = model.Letra.ToUpper(),
                Capacidad      = model.Capacidad,
                ProfesorJefeID = model.ProfesorJefeID,
                IsActive       = true,
                CreatedDate    = DateTime.UtcNow,
                CreatedBy      = usuario
            };
            _dbContext.tbl_Curso.Add(entidad);
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> UpdateCursoAsync(CursoFormViewModel model, string usuario)
        {
            var entidad = await _dbContext.tbl_Curso.FindAsync(model.ID);
            if (entidad == null) return 0;

            var existe = await _dbContext.tbl_Curso
                .AnyAsync(c => c.AnioEscolarID == model.AnioEscolarID
                            && c.GradoID == model.GradoID
                            && c.Letra.ToUpper() == model.Letra.ToUpper()
                            && c.IsActive
                            && c.ID != model.ID);
            if (existe) return -1;

            entidad.GradoID        = model.GradoID;
            entidad.Letra          = model.Letra.ToUpper();
            entidad.Capacidad      = model.Capacidad;
            entidad.ProfesorJefeID = model.ProfesorJefeID;
            await _dbContext.SaveChangesAsync();
            return entidad.ID;
        }

        public async Task<int> EliminarCursoAsync(int cursoId, string usuario)
        {
            var entidad = await _dbContext.tbl_Curso.FindAsync(cursoId);
            if (entidad == null) return 0;

            // No eliminar si tiene alumnos matriculados activos
            var tieneAlumnos = await _dbContext.tbl_Matricula
                .AnyAsync(m => m.CursoID == cursoId && m.IsActive);
            if (tieneAlumnos) return -1;

            entidad.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return cursoId;
        }

        // ?? Lookups ???????????????????????????????????????????????????????

        public async Task<List<SelectItemViewModel>> GetProfesoresAsync()
        {
            return await _dbContext.tbl_ProfesorJefe
                .Where(p => p.IsActive && p.Vigente)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .Select(p => new SelectItemViewModel
                {
                    ID    = p.ID,
                    Texto = p.Nombre + " " + p.Apellido
                })
                .ToListAsync();
        }

        public async Task<List<SelectItemViewModel>> GetGradosAsync()
        {
            return await _dbContext.tbl_Grado
                .Where(g => g.IsActive)
                .OrderBy(g => g.Orden)
                .Select(g => new SelectItemViewModel { ID = g.ID, Texto = g.Nombre })
                .ToListAsync();
        }
    }
}
