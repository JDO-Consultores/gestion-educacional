using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GestionColegios.Services
{
    public class HomeService : BaseServices, IHomeService
    {
        public HomeService(
            Entities dbContext,
            IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
            var alumnos = _dbContext.tbl_Alumno.Where(a => a.IsActive);

            var totalAlumnos     = await alumnos.CountAsync();
            var alumnosVigentes  = await alumnos.CountAsync(a => a.EstadoAlumnoID == 1);
            var alumnosRetirados = await alumnos.CountAsync(a => a.EstadoAlumnoID == 2);
            var alumnosConPIE    = await alumnos.CountAsync(a => a.TienePIE);

            // Año escolar activo
            var anioActivo = await GetAnioEscolarActivoAsync();

            int anioId  = anioActivo?.ID ?? 0;
            int anioNum = anioActivo?.Anio ?? 0;

            // Matrículas del año activo
            var matsAnio = await _dbContext.tbl_Matricula
                .Where(m => m.IsActive && m.AnioEscolarID == anioId)
                .Select(m => new { m.EstadoMatriculaID, m.tbl_EstadoMatricula.Nombre, m.EsAlumnoNuevo })
                .ToListAsync();

            var vigentes        = matsAnio.Count(m => m.Nombre == "Matriculado");
            var preMatriculados = matsAnio.Count(m => m.Nombre == "Pre-Matriculado");
            var anuladas        = matsAnio.Count(m => m.Nombre == "Anulada");
            var nuevos          = matsAnio.Count(m => m.EsAlumnoNuevo);

            // Alumnos con documentos obligatorios pendientes
            var tiposObligIDs = await _dbContext.tbl_TipoDocumento
                .Where(t => t.IsActive && !t.EsAnexo && t.Obligatorio)
                .Select(t => t.ID)
                .ToListAsync();

            int conDocsPendientes = 0;
            if (anioId > 0 && tiposObligIDs.Any())
            {
                var alumnosConMatricula = await _dbContext.tbl_Matricula
                    .Where(m => m.IsActive && m.AnioEscolarID == anioId)
                    .Select(m => m.AlumnoID)
                    .Distinct()
                    .ToListAsync();

                foreach (var alumnoId in alumnosConMatricula)
                {
                    var cargados = await _dbContext.tbl_DocumentoAlumno
                        .Where(d => d.AlumnoID == alumnoId
                                 && d.AnioEscolarID == anioId
                                 && d.IsActive)
                        .Select(d => d.TipoDocumentoID)
                        .ToListAsync();

                    if (tiposObligIDs.Any(id => !cargados.Contains(id)))
                        conDocsPendientes++;
                }
            }

            return new DashboardViewModel
            {
                TotalAlumnos                   = totalAlumnos,
                AlumnosVigentes                = alumnosVigentes,
                AlumnosRetirados               = alumnosRetirados,
                AlumnosConPIE                  = alumnosConPIE,
                AnioEscolarActivo              = anioNum,
                MatriculasVigentes             = vigentes,
                MatriculasPreMatriculadas      = preMatriculados,
                MatriculasAnuladas             = anuladas,
                MatriculasAlumnosNuevos        = nuevos,
                AlumnosConDocumentosPendientes = conDocsPendientes,
                AlumnosPorCurso                = await GetAlumnosPorCursoAsync(anioId),
                ActividadReciente              = await GetActividadRecienteAsync(15)
            };
        }

        public async Task<List<AlumnosPorCursoViewModel>> GetAlumnosPorCursoAsync(int anioEscolarId)
        {
            if (anioEscolarId == 0) return new List<AlumnosPorCursoViewModel>();

            var cursos = await _dbContext.tbl_Matricula
                .Where(m => m.IsActive && m.AnioEscolarID == anioEscolarId)
                .GroupBy(m => new { m.CursoID, m.tbl_Curso.tbl_Grado.Nombre, m.tbl_Curso.tbl_Grado.Orden, m.tbl_Curso.Letra })
                .Select(g => new AlumnosPorCursoViewModel
                {
                    CursoID  = g.Key.CursoID,
                    Curso    = g.Key.Nombre + " " + g.Key.Letra,
                    Total    = g.Count(),
                    Vigentes = g.Count(m => m.tbl_EstadoMatricula.Nombre == "Vigente")
                })
                .OrderBy(c => c.Curso)
                .ToListAsync();

            return cursos;
        }

        public async Task<List<ActividadRecienteViewModel>> GetActividadRecienteAsync(int cantidad)
        {
            return await _dbContext.tbl_LogActividad
                .OrderByDescending(l => l.FechaAccion)
                .Take(cantidad)
                .Select(l => new ActividadRecienteViewModel
                {
                    Entidad     = l.Entidad,
                    EntidadID   = l.EntidadID,
                    Accion      = l.Accion,
                    Usuario     = l.Usuario,
                    FechaAccion = l.FechaAccion,
                    Detalle     = l.Detalle
                })
                .ToListAsync();
        }
    }
}
