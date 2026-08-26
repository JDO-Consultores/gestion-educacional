using GestionColegios.Interfaces;
using GestionColegios.Helpers;
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
    public class MatriculaService : BaseServices, IMatriculaService
    {
        private readonly IDocumentoService _documentoService;

        public MatriculaService(
            Entities dbContext,
            IMapperService mapperService,
            IDocumentoService documentoService) : base(dbContext, mapperService)
        {
            _documentoService = documentoService;
        }

        public async Task<DataSourceResult> GetMatriculasPorAlumnoAsync(int alumnoId, DataSourceRequest request)
        {
            var lista = await _dbContext.tbl_Matricula
                .Where(m => m.AlumnoID == alumnoId && m.IsActive)
                .OrderByDescending(m => m.tbl_AnioEscolar.Anio)
                .Select(m => new MatriculaViewModel
                {
                    ID                   = m.ID,
                    AlumnoID             = m.AlumnoID,
                    AlumnoNombreCompleto = m.tbl_Alumno.ApellidoPaterno + " " + m.tbl_Alumno.ApellidoMaterno + ", " + m.tbl_Alumno.Nombres,
                    AlumnoRut            = m.tbl_Alumno.Rut,
                    CursoID              = m.CursoID,
                    CursoNombre          = m.tbl_Curso.tbl_Grado.Nombre + " " + m.tbl_Curso.Letra,
                    AnioEscolarID        = m.AnioEscolarID,
                    AnioEscolar          = m.tbl_AnioEscolar.Anio,
                    NroMatricula         = m.NroMatricula,
                    NroMatriculaAnterior = m.NroMatriculaAnterior,
                    FechaMatricula       = m.FechaMatricula,
                    EstadoMatriculaID    = m.EstadoMatriculaID,
                    EstadoMatricula      = m.tbl_EstadoMatricula.Nombre,
                    EnListaEspera        = m.tbl_EstadoMatricula.Nombre == "Lista de Espera",
                    EsAlumnoNuevo        = m.EsAlumnoNuevo,
                    Observacion          = m.Observacion,
                    CreatedDate          = m.CreatedDate,
                    CreatedBy            = m.CreatedBy,
                    ModifiedDate         = m.ModifiedDate,
                    ModifiedBy           = m.ModifiedBy
                })
                .ToListAsync();

            // Aplicar formateo de RUT en memoria
            foreach (var item in lista)
                item.AlumnoRut = RutHelper.Formatear(item.AlumnoRut);

            return lista
                .AsQueryable()
                .ToDataSourceResult(
                    request.Take,
                    request.Skip,
                    request.Sort,
                    request.Filter,
                    request.Aggregate,
                    request.Group);
        }

        public async Task<DataSourceResult> GetMatriculasAsync(int? anioEscolarId, DataSourceRequest request)
        {
            var query = _dbContext.tbl_Matricula
                .Where(m => m.IsActive);

            if (anioEscolarId.HasValue)
                query = query.Where(m => m.AnioEscolarID == anioEscolarId.Value);

            var lista = await query
                .OrderByDescending(m => m.tbl_AnioEscolar.Anio)
                .ThenBy(m => m.tbl_Alumno.ApellidoPaterno)
                .Select(m => new MatriculaViewModel
                {
                    ID                   = m.ID,
                    AlumnoID             = m.AlumnoID,
                    AlumnoNombreCompleto = m.tbl_Alumno.ApellidoPaterno + " " + m.tbl_Alumno.ApellidoMaterno + ", " + m.tbl_Alumno.Nombres,
                    AlumnoRut            = m.tbl_Alumno.Rut,
                    CursoID              = m.CursoID,
                    CursoNombre          = m.tbl_Curso.tbl_Grado.Nombre + " " + m.tbl_Curso.Letra,
                    AnioEscolarID        = m.AnioEscolarID,
                    AnioEscolar          = m.tbl_AnioEscolar.Anio,
                    NroMatricula         = m.NroMatricula,
                    NroMatriculaAnterior = m.NroMatriculaAnterior,
                    FechaMatricula       = m.FechaMatricula,
                    EstadoMatriculaID    = m.EstadoMatriculaID,
                    EstadoMatricula      = m.tbl_EstadoMatricula.Nombre,
                    EnListaEspera        = m.tbl_EstadoMatricula.Nombre == "Lista de Espera",
                    EsAlumnoNuevo        = m.EsAlumnoNuevo,
                    Observacion          = m.Observacion,
                    CreatedDate          = m.CreatedDate,
                    CreatedBy            = m.CreatedBy,
                    ModifiedDate         = m.ModifiedDate,
                    ModifiedBy           = m.ModifiedBy
                })
                .ToListAsync();

            foreach (var item in lista)
                item.AlumnoRut = RutHelper.Formatear(item.AlumnoRut);

            return lista.AsQueryable().ToDataSourceResult(
                request.Take, request.Skip,
                request.Sort, request.Filter,
                request.Aggregate, request.Group);
        }

        public async Task<MatriculaResumenAnioViewModel> GetResumenAnioAsync(int? anioEscolarId)
        {
            // Si no se indica año, usar el más reciente activo
            int anioId;
            int anio;
            if (anioEscolarId.HasValue)
            {
                var ae = await _dbContext.tbl_AnioEscolar.FindAsync(anioEscolarId.Value);
                anioId = ae?.ID ?? 0;
                anio   = ae?.Anio ?? 0;
            }
            else
            {
                var ae = await GetAnioEscolarActivoAsync();
                anioId = ae?.ID ?? 0;
                anio   = ae?.Anio ?? 0;
            }

            var matriculas = await _dbContext.tbl_Matricula
                .Where(m => m.IsActive && m.AnioEscolarID == anioId)
                .Select(m => new { m.EstadoMatriculaID, m.tbl_EstadoMatricula.Nombre, m.EsAlumnoNuevo })
                .ToListAsync();

            var aniosDisponibles = await _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.Anio)
                .Select(a => new SelectItemViewModel { ID = a.ID, Texto = a.Anio.ToString() })
                .ToListAsync();

            return new MatriculaResumenAnioViewModel
            {
                AnioEscolarID     = anioId,
                Anio              = anio,
                TotalMatriculas   = matriculas.Count,
                Vigentes          = matriculas.Count(m => m.Nombre == "Matriculado"),
                PreMatriculados   = matriculas.Count(m => m.Nombre == "Pre-Matriculado"),
                Anuladas          = matriculas.Count(m => m.Nombre == "Anulada"),
                EnListaEspera     = matriculas.Count(m => m.Nombre == "Lista de Espera"),
                AlumnosNuevos     = matriculas.Count(m => m.EsAlumnoNuevo),
                AlumnosAntiguos   = matriculas.Count(m => !m.EsAlumnoNuevo),
                AniosDisponibles  = aniosDisponibles
            };
        }

        public async Task<MatriculaViewModel> GetMatriculaAsync(int matriculaId)
        {
            var m = await _dbContext.tbl_Matricula
                .Include(x => x.tbl_Alumno)
                .Include(x => x.tbl_Curso.tbl_Grado)
                .Include(x => x.tbl_AnioEscolar)
                .Include(x => x.tbl_EstadoMatricula)
                .FirstOrDefaultAsync(x => x.ID == matriculaId && x.IsActive);

            if (m == null) return null;

            return new MatriculaViewModel
            {
                ID                   = m.ID,
                AlumnoID             = m.AlumnoID,
                AlumnoNombreCompleto = m.tbl_Alumno.ApellidoPaterno + " " + m.tbl_Alumno.ApellidoMaterno + ", " + m.tbl_Alumno.Nombres,
                AlumnoRut            = RutHelper.Formatear(m.tbl_Alumno.Rut),
                CursoID              = m.CursoID,
                CursoNombre          = m.tbl_Curso.tbl_Grado.Nombre + " " + m.tbl_Curso.Letra,
                AnioEscolarID        = m.AnioEscolarID,
                AnioEscolar          = m.tbl_AnioEscolar.Anio,
                NroMatricula         = m.NroMatricula,
                NroMatriculaAnterior = m.NroMatriculaAnterior,
                FechaMatricula       = m.FechaMatricula,
                EstadoMatriculaID    = m.EstadoMatriculaID,
                EstadoMatricula      = m.tbl_EstadoMatricula.Nombre,
                EnListaEspera        = m.tbl_EstadoMatricula.Nombre == "Lista de Espera",
                EsAlumnoNuevo        = m.EsAlumnoNuevo,
                Observacion          = m.Observacion,
                CreatedDate          = m.CreatedDate,
                CreatedBy            = m.CreatedBy,
                ModifiedDate         = m.ModifiedDate,
                ModifiedBy           = m.ModifiedBy
            };
        }

        public async Task<MatriculaFormViewModel> GetFormDataAsync(int alumnoId)
        {
            var alumno = await _dbContext.tbl_Alumno
                .FirstOrDefaultAsync(a => a.ID == alumnoId && a.IsActive);

            if (alumno == null) return null;

            var anios = await _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive && !a.Cerrado)
                .OrderByDescending(a => a.Anio)
                .Select(a => new SelectItemViewModel { ID = a.ID, Texto = a.Anio.ToString() })
                .ToListAsync();

            var estados = await _dbContext.tbl_EstadoMatricula
                .Where(e => e.IsActive)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToListAsync();

            // Cursos del año escolar activo (se actualizan vía Ajax al cambiar año)
            var anioActivoObj = await GetAnioEscolarActivoAsync();
            var anioActualId  = anios.FirstOrDefault(a => a.ID == anioActivoObj?.ID)?.ID
                                ?? anios.Select(a => a.ID).FirstOrDefault();
            var cursos = await GetCursosPorAnioAsync(anioActualId);

            // Determinar si es alumno nuevo (nunca ha tenido matrícula)
            var tieneMatriculaPrevia = await _dbContext.tbl_Matricula
                .AnyAsync(m => m.AlumnoID == alumnoId);

            return new MatriculaFormViewModel
            {
                AlumnoID             = alumno.ID,
                AlumnoNombreCompleto = alumno.ApellidoPaterno + " " + alumno.ApellidoMaterno + ", " + alumno.Nombres,
                AlumnoRut            = RutHelper.Formatear(alumno.Rut),
                EsAlumnoNuevo        = !tieneMatriculaPrevia,
                AniosEscolares       = anios,
                Cursos               = cursos,
                EstadosMatricula     = estados,
                Matricula            = new MatriculaViewModel
                {
                    AlumnoID          = alumno.ID,
                    FechaMatricula    = DateTime.Today,
                    EstadoMatriculaID = 1,
                    EsAlumnoNuevo     = !tieneMatriculaPrevia
                }
            };
        }

        public async Task<MatriculaResultado> CreateMatriculaAsync(MatriculaViewModel model, string createdBy)
        {
            // Validar que el alumno exista antes de cualquier operación
            var alumno = await _dbContext.tbl_Alumno.FindAsync(model.AlumnoID);
            if (alumno == null || !alumno.IsActive)
                return new MatriculaResultado { MatriculaID = 0 };

            // Validar duplicado por año
            if (await ExisteMatriculaEnAnioAsync(model.AlumnoID, model.AnioEscolarID))
                return new MatriculaResultado { MatriculaID = -1 };

            // Bloqueo por "Matrícula Cancelada": si la última matrícula del alumno quedó marcada
            // como cancelada (al cierre de año), no puede matricularse hasta que un supervisor
            // levante la condición. Si ya fue autorizada (MatriculaCancelada=false) puede continuar.
            var ultimaMatricula = await _dbContext.tbl_Matricula
                .Where(m => m.AlumnoID == model.AlumnoID)
                .OrderByDescending(m => m.ID)
                .Select(m => new { m.MatriculaCancelada })
                .FirstOrDefaultAsync();
            if (ultimaMatricula != null && ultimaMatricula.MatriculaCancelada)
                return new MatriculaResultado { MatriculaID = -3 };

            // Determinar si es alumno nuevo (no tiene ninguna matrícula previa)
            var esAlumnoNuevo = !await _dbContext.tbl_Matricula
                .AnyAsync(m => m.AlumnoID == model.AlumnoID);

            // ¿El alumno estaba retirado? (EstadoAlumnoID 2 = Retirado). Si es así, este es un
            // reingreso: se le asigna un nuevo número de matrícula y se conserva el anterior.
            var esReingreso = alumno.EstadoAlumnoID == 2;

            // Recuperar el último número de matrícula del alumno (para conservarlo como histórico)
            var nroMatriculaAnterior = await _dbContext.tbl_Matricula
                .Where(m => m.AlumnoID == model.AlumnoID)
                .OrderByDescending(m => m.ID)
                .Select(m => m.NroMatricula)
                .FirstOrDefaultAsync();

            // Generar número de matrícula:
            //  - Reingreso  -> nuevo número (no reutiliza el base), conservando el anterior como histórico.
            //  - Otros años -> mismo número base por alumno, cambia el año.
            var nroMatricula = await GenerarNroMatriculaAsync(model.AlumnoID, model.AnioEscolarID, esReingreso);

            // Verificar si los documentos obligatorios ya están cargados
            var verificacion = await _documentoService.VerificarDocumentosObligatoriosAsync(
                model.AlumnoID, model.AnioEscolarID);

            // Control de cupos: si el curso ya alcanzó su capacidad, la matrícula queda en
            // estado "Lista de Espera"; tiene prioridad sobre cualquier otro estado.
            var enListaEspera = !await HayCupoDisponibleAsync(model.CursoID, 0);

            // Determinar estado de la matrícula
            int estadoMatriculaId;
            if (enListaEspera)
            {
                estadoMatriculaId = await GetEstadoMatriculaIdAsync("Lista de Espera", model.EstadoMatriculaID);
            }
            else if (!verificacion.TodosObligatoriosCargados)
            {
                // Faltan documentos obligatorios -> Pre-matriculado
                estadoMatriculaId = await GetEstadoMatriculaIdAsync("Pre-Matriculado", model.EstadoMatriculaID);
            }
            else
            {
                estadoMatriculaId = model.EstadoMatriculaID;
            }

            var matricula = new tbl_Matricula
            {
                AlumnoID             = model.AlumnoID,
                CursoID              = model.CursoID,
                AnioEscolarID        = model.AnioEscolarID,
                NroMatricula         = nroMatricula,
                NroMatriculaAnterior = esReingreso ? nroMatriculaAnterior : null,
                FechaMatricula       = model.FechaMatricula,
                EstadoMatriculaID    = estadoMatriculaId,
                EsAlumnoNuevo        = esAlumnoNuevo,
                Observacion          = model.Observacion,
                IsActive             = true,
                CreatedDate          = DateTime.UtcNow,
                CreatedBy            = createdBy
            };

            _dbContext.tbl_Matricula.Add(matricula);

            // Actualizar estado del alumno a Vigente (ID=1)
            alumno.EstadoAlumnoID = 1; // Vigente
            alumno.ModifiedDate   = DateTime.UtcNow;
            alumno.ModifiedBy     = createdBy;

            await _dbContext.SaveChangesAsync();

            var curso = await _dbContext.tbl_Curso
                .Include(c => c.tbl_Grado)
                .FirstOrDefaultAsync(c => c.ID == matricula.CursoID);
            var anio = await _dbContext.tbl_AnioEscolar.FindAsync(matricula.AnioEscolarID);

            var detalleLog = $"N°: {nroMatricula} | Curso: {curso?.tbl_Grado?.Nombre} {curso?.Letra} | Año: {anio?.Anio}";
            if (esReingreso && !string.IsNullOrEmpty(nroMatriculaAnterior))
                detalleLog += $" | Reingreso (N° anterior: {nroMatriculaAnterior})";
            if (enListaEspera)
                detalleLog += " | LISTA DE ESPERA (sin cupos)";

            await RegistrarLogAsync("Matrícula", model.AlumnoID, "REGISTRÓ MATRÍCULA",
                createdBy, detalleLog);

            return new MatriculaResultado
            {
                MatriculaID          = matricula.ID,
                EnListaEspera        = enListaEspera,
                NroMatricula         = nroMatricula,
                NroMatriculaAnterior = esReingreso ? nroMatriculaAnterior : null
            };
        }

        public async Task<int> UpdateMatriculaAsync(MatriculaViewModel model, string modifiedBy)
        {
            var mat = await _dbContext.tbl_Matricula.FindAsync(model.ID);
            if (mat == null) return 0;

            if (mat.AnioEscolarID != model.AnioEscolarID &&
                await ExisteMatriculaEnAnioAsync(model.AlumnoID, model.AnioEscolarID))
                return -1;

            mat.CursoID           = model.CursoID;
            mat.AnioEscolarID     = model.AnioEscolarID;
            mat.FechaMatricula    = model.FechaMatricula;
            mat.EstadoMatriculaID = model.EstadoMatriculaID;
            mat.Observacion       = model.Observacion;
            mat.ModifiedDate      = DateTime.UtcNow;
            mat.ModifiedBy        = modifiedBy;

            await _dbContext.SaveChangesAsync();

            // Si quedan todos los documentos obligatorios completos, promover a Matriculado automáticamente
            await ActualizarEstadoSegunDocumentosAsync(mat.AlumnoID, mat.AnioEscolarID, modifiedBy);

            var cursoEdit = await _dbContext.tbl_Curso
                .Include(c => c.tbl_Grado)
                .FirstOrDefaultAsync(c => c.ID == mat.CursoID);
            var anioEdit = await _dbContext.tbl_AnioEscolar.FindAsync(mat.AnioEscolarID);
            await RegistrarLogAsync("Matrícula", mat.AlumnoID, "EDITÓ MATRÍCULA",
                modifiedBy,
                $"N°: {mat.NroMatricula} | Curso: {cursoEdit?.tbl_Grado?.Nombre} {cursoEdit?.Letra} | Año: {anioEdit?.Anio}");

            return mat.ID;
        }

        public async Task<int> AnularMatriculaAsync(int matriculaId, string observacion, string modifiedBy)
        {
            var mat = await _dbContext.tbl_Matricula.FindAsync(matriculaId);
            if (mat == null) return 0;

            var cursoIdLiberado = mat.CursoID;

            mat.EstadoMatriculaID = 2;
            mat.Observacion       = observacion;
            mat.ModifiedDate      = DateTime.UtcNow;
            mat.ModifiedBy        = modifiedBy;

            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Matrícula", mat.AlumnoID, "ANULÓ MATRÍCULA",
                modifiedBy,
                $"N°: {mat.NroMatricula}{(string.IsNullOrWhiteSpace(observacion) ? "" : " | Obs: " + observacion)}");

            // Al liberarse un cupo, correr la lista de espera del curso
            await CorrerListaEsperaAsync(cursoIdLiberado, modifiedBy);

            return matriculaId;
        }

        /// <summary>
        /// Cuando se libera un cupo en el curso (por anulación/retiro), promueve a la
        /// siguiente matrícula en "Lista de Espera" (la más antigua) al estado
        /// "Pre-Matriculado". Si además tiene los documentos obligatorios completos,
        /// se promueve automáticamente a "Matriculado". Devuelve la cantidad de
        /// matrículas que avanzaron.
        /// </summary>
        public async Task<int> CorrerListaEsperaAsync(int cursoId, string modifiedBy)
        {
            var curso = await _dbContext.tbl_Curso
                .Where(c => c.ID == cursoId)
                .Select(c => new { c.Capacidad })
                .FirstOrDefaultAsync();

            // Sin capacidad definida no hay control de cupos -> nada que correr
            if (curso == null || !curso.Capacidad.HasValue) return 0;

            var estadoPreMatriculadoId = await GetEstadoMatriculaIdAsync("Pre-Matriculado", 0);
            if (estadoPreMatriculadoId == 0) return 0;

            int avanzados = 0;

            // Avanzar tantos como cupos haya disponibles, respetando el orden de la lista de espera
            while (await HayCupoDisponibleAsync(cursoId, 0))
            {
                var siguiente = await _dbContext.tbl_Matricula
                    .Where(m => m.CursoID == cursoId
                             && m.IsActive
                             && m.tbl_EstadoMatricula.Nombre == "Lista de Espera")
                    .OrderBy(m => m.FechaMatricula)
                    .ThenBy(m => m.ID)
                    .FirstOrDefaultAsync();

                if (siguiente == null) break; // no hay más en lista de espera

                siguiente.EstadoMatriculaID = estadoPreMatriculadoId;
                siguiente.ModifiedDate      = DateTime.UtcNow;
                siguiente.ModifiedBy        = modifiedBy;
                await _dbContext.SaveChangesAsync();

                await RegistrarLogAsync("Matrícula", siguiente.AlumnoID, "AVANCE LISTA DE ESPERA",
                    modifiedBy,
                    $"N°: {siguiente.NroMatricula} | Se liberó un cupo. Estado actualizado a Pre-Matriculado.");

                // Si ya tiene todos los documentos obligatorios, promover a Matriculado
                await ActualizarEstadoSegunDocumentosAsync(
                    siguiente.AlumnoID, siguiente.AnioEscolarID, modifiedBy);

                avanzados++;
            }

            return avanzados;
        }

        public async Task<bool> ExisteMatriculaEnAnioAsync(int alumnoId, int anioEscolarId)
        {
            return await _dbContext.tbl_Matricula
                .AnyAsync(m => m.AlumnoID == alumnoId
                            && m.AnioEscolarID == anioEscolarId
                            && m.IsActive);
        }

        // ── Helpers internos ─────────────────────────────────────────────

        private async Task<List<SelectItemViewModel>> GetCursosPorAnioAsync(int anioEscolarId)
        {
            return await _dbContext.tbl_Curso
                .Where(c => c.AnioEscolarID == anioEscolarId && c.IsActive)
                .OrderBy(c => c.tbl_Grado.Orden)
                .ThenBy(c => c.Letra)
                .Select(c => new SelectItemViewModel
                {
                    ID    = c.ID,
                    Texto = c.tbl_Grado.Nombre + " " + c.Letra
                })
                .ToListAsync();
        }

        private async Task<string> GenerarNroMatriculaAsync(int alumnoId, int anioEscolarId, bool esReingreso = false)
        {
            var anio = await _dbContext.tbl_AnioEscolar
                .Where(a => a.ID == anioEscolarId)
                .Select(a => a.Anio)
                .FirstOrDefaultAsync();

            // Si el alumno ya tuvo matrícula en otro año, reutilizar su número base.
            // En un reingreso (alumno retirado que vuelve) se fuerza un número nuevo y el
            // anterior queda como histórico.
            var matriculaAnterior = esReingreso
                ? null
                : await _dbContext.tbl_Matricula
                    .Where(m => m.AlumnoID == alumnoId)
                    .OrderBy(m => m.ID)
                    .Select(m => m.NroMatricula)
                    .FirstOrDefaultAsync();

            int numeroBase;

            if (matriculaAnterior != null)
            {
                // Extraer el número base del formato "YYYY-NNNNN"
                var partes = matriculaAnterior.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[1], out int nBase))
                    numeroBase = nBase;
                else
                    numeroBase = await ObtenerSiguienteNumeroGlobalAsync();
            }
            else
            {
                // Nuevo alumno o reingreso: asignar el siguiente número global disponible
                numeroBase = await ObtenerSiguienteNumeroGlobalAsync();
            }

            return string.Format("{0}-{1:D5}", anio, numeroBase);
        }

        private async Task<int> ObtenerSiguienteNumeroGlobalAsync()
        {
            // Busca el mayor número base usado en todas las matrículas
            var numeros = await _dbContext.tbl_Matricula
                .Select(m => m.NroMatricula)
                .ToListAsync();

            int max = numeros
                .Select(n => {
                    var p = n?.Split('-');
                    return p != null && p.Length == 2 && int.TryParse(p[1], out int v) ? v : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            return max + 1;
        }

        /// <summary>
        /// Indica si el curso tiene cupos disponibles según su capacidad.
        /// Si la capacidad es nula (sin límite definido) siempre devuelve true.
        /// Las matrículas anuladas o en lista de espera no consumen cupo.
        /// </summary>
        private async Task<bool> HayCupoDisponibleAsync(int cursoId, int matriculaIdExcluir)
        {
            var curso = await _dbContext.tbl_Curso
                .Where(c => c.ID == cursoId)
                .Select(c => new { c.Capacidad })
                .FirstOrDefaultAsync();

            // Sin capacidad definida -> sin límite de cupos
            if (curso == null || !curso.Capacidad.HasValue)
                return true;

            var ocupados = await _dbContext.tbl_Matricula
                .CountAsync(m => m.CursoID == cursoId
                              && m.IsActive
                              && m.ID != matriculaIdExcluir
                              && m.tbl_EstadoMatricula.Nombre != "Anulada"
                              && m.tbl_EstadoMatricula.Nombre != "Lista de Espera");

            return ocupados < curso.Capacidad.Value;
        }

        /// <summary>
        /// Devuelve el ID del estado de matrícula con el nombre indicado.
        /// Si no existe, retorna el valor por defecto recibido.
        /// </summary>
        private async Task<int> GetEstadoMatriculaIdAsync(string nombre, int estadoPorDefecto)
        {
            var id = await _dbContext.tbl_EstadoMatricula
                .Where(e => e.IsActive && e.Nombre == nombre)
                .Select(e => e.ID)
                .FirstOrDefaultAsync();
            return id > 0 ? id : estadoPorDefecto;
        }

        public async Task<bool> ActualizarEstadoSegunDocumentosAsync(
            int alumnoId, int anioEscolarId, string modifiedBy)
        {
            var matricula = await _dbContext.tbl_Matricula
                .FirstOrDefaultAsync(m => m.AlumnoID == alumnoId
                                       && m.AnioEscolarID == anioEscolarId
                                       && m.IsActive);
            if (matricula == null) return false;

            // Solo actuar si está en estado Pre-matriculado o Vigente (no Anulada).
            // Las matrículas en Lista de Espera no se promueven aunque tengan documentos
            // completos: deben esperar a que se libere un cupo en el curso.
            var estadoActual = await _dbContext.tbl_EstadoMatricula.FindAsync(matricula.EstadoMatriculaID);
            if (estadoActual == null
                || estadoActual.Nombre == "Anulada"
                || estadoActual.Nombre == "Lista de Espera") return false;

            var verificacion = await _documentoService.VerificarDocumentosObligatoriosAsync(alumnoId, anioEscolarId);
            if (!verificacion.TodosObligatoriosCargados) return false;

            // Promover al estado Vigente (ID=1) o el primero que no sea Pre-matriculado/Anulada
            var estadoVigente = await _dbContext.tbl_EstadoMatricula
                .Where(e => e.IsActive && e.Nombre == "Matriculado")
                .Select(e => e.ID)
                .FirstOrDefaultAsync();

            if (estadoVigente == 0) return false;

            matricula.EstadoMatriculaID = estadoVigente;
            matricula.ModifiedDate      = DateTime.UtcNow;
            matricula.ModifiedBy        = modifiedBy;
            await _dbContext.SaveChangesAsync();

            await RegistrarLogAsync("Matrícula", alumnoId, "MATRICULA COMPLETADA",
                modifiedBy,
                $"N°: {matricula.NroMatricula} | Todos los documentos obligatorios fueron cargados. Estado actualizado a Vigente.");

            return true;
        }

        public async Task<List<SelectItemViewModel>> GetEstadosMatriculaAsync()
        {
            return await _dbContext.tbl_EstadoMatricula
                .Where(e => e.IsActive)
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToListAsync();
        }
    }
}