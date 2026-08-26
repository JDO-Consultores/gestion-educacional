using GestionColegios.Model;
using GestionColegios.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class LookupController : DefaultController
    {
        private readonly Entities _dbContext;

        public LookupController(Entities dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.CommandTimeout = 180;
        }

        // GET: /Lookup/CursosPorAnio?anioEscolarId=1
        public JsonResult CursosPorAnio(int anioEscolarId)
        {
            var cursos = _dbContext.tbl_Curso
                .Where(c => c.AnioEscolarID == anioEscolarId && c.IsActive)
                .OrderBy(c => c.tbl_Grado.Orden)
                .ThenBy(c => c.Letra)
                .Select(c => new SelectItemViewModel { ID = c.ID, Texto = c.tbl_Grado.Nombre + " " + c.Letra })
                .ToList();

            return Json(cursos, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/CursosPorAnioConCupos?anioEscolarId=1&matriculaId=0
        public JsonResult CursosPorAnioConCupos(int anioEscolarId, int matriculaId = 0)
        {
            var cursos = _dbContext.tbl_Curso
                .Where(c => c.AnioEscolarID == anioEscolarId && c.IsActive)
                .OrderBy(c => c.tbl_Grado.Orden)
                .ThenBy(c => c.Letra)
                .Select(c => new
                {
                    ID         = c.ID,
                    Texto      = c.tbl_Grado.Nombre + " " + c.Letra,
                    Capacidad  = c.Capacidad,
                    Matriculados = c.tbl_Matricula
                        .Count(m => m.IsActive
                                 && m.ID != matriculaId
                                 && m.tbl_EstadoMatricula.Nombre != "Anulada"
                                 && m.tbl_EstadoMatricula.Nombre != "Lista de Espera")
                })
                .ToList()
                .Select(c => new
                {
                    c.ID,
                    c.Texto,
                    c.Capacidad,
                    c.Matriculados,
                    Disponibles = c.Capacidad.HasValue ? c.Capacidad.Value - c.Matriculados : (int?)null
                });

            return Json(cursos, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/CausalesRetiro
        public JsonResult CausalesRetiro()
        {
            var causales = _dbContext.tbl_CausalRetiro
                .Where(c => c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectItemViewModel { ID = c.ID, Texto = c.Nombre })
                .ToList();

            return Json(causales, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/Sexos
        public JsonResult Sexos()
        {
            var items = _dbContext.tbl_Sexo
                .Where(s => s.IsActive)
                .OrderBy(s => s.Nombre)
                .Select(s => new SelectItemViewModel { ID = s.ID, Texto = s.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/Nacionalidades
        public JsonResult Nacionalidades()
        {
            var items = _dbContext.tbl_Nacionalidad
                .Where(n => n.IsActive)
                .OrderBy(n => n.Nombre)
                .Select(n => new SelectItemViewModel { ID = n.ID, Texto = n.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/SistemasSalud
        public JsonResult SistemasSalud()
        {
            var items = _dbContext.tbl_SistemaSalud
                .Where(s => s.IsActive)
                .OrderBy(s => s.Nombre)
                .Select(s => new SelectItemViewModel { ID = s.ID, Texto = s.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/EstadosAlumno
        public JsonResult EstadosAlumno()
        {
            var items = _dbContext.tbl_EstadoAlumno
                .Where(e => e.IsActive)
                .OrderBy(e => e.Nombre)
                .Select(e => new SelectItemViewModel { ID = e.ID, Texto = e.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/Regiones
        public JsonResult Regiones()
        {
            var items = _dbContext.tbl_Region
                .Where(r => r.IsActive)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectItemViewModel { ID = r.ID, Texto = r.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/ComunasPorRegion?regionId=1
        public JsonResult ComunasPorRegion(int regionId)
        {
            var items = _dbContext.tbl_Comuna
                .Where(c => c.RegionID == regionId && c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectItemViewModel { ID = c.ID, Texto = c.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/TiposAlergia
        public JsonResult TiposAlergia()
        {
            var items = _dbContext.tbl_TipoAlergia
                .OrderBy(t => t.ID)
                .Select(t => new SelectItemViewModel { ID = t.ID, Texto = t.Nombre })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/CatalogoAlergias?tipoAlergiaId=2
        public JsonResult CatalogoAlergias(int tipoAlergiaId)
        {
            var items = _dbContext.tbl_CatalogoAlergia
                .Where(c => c.TipoAlergiaID == tipoAlergiaId && c.IsActive)
                .OrderBy(c => c.Orden)
                .Select(c => new
                {
                    c.ID,
                    Texto = c.Nombre,
                    c.RequiereDetalle
                })
                .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/TiposDiscapacidad
        public JsonResult TiposDiscapacidad()        {
            var items = _dbContext.tbl_TipoDiscapacidad
                .Where(t => t.IsActive)
                .OrderBy(t => t.Nombre)
                .Select(t => new { ID = t.ID, Texto = t.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/Parentescos
        public JsonResult Parentescos()
        {
            var items = _dbContext.tbl_Parentesco
                .Where(p => p.IsActive)
                .OrderBy(p => p.Nombre)
                .Select(p => new { ID = p.ID, Texto = p.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/NivelesEducacionales
        public JsonResult NivelesEducacionales()
        {
            var items = _dbContext.tbl_NivelEducacional
                .Where(n => n.IsActive)
                .OrderBy(n => n.Nombre)
                .Select(n => new { ID = n.ID, Texto = n.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/SituacionesLaborales
        public JsonResult SituacionesLaborales()
        {
            var items = _dbContext.tbl_SituacionLaboral
                .Where(s => s.IsActive)
                .OrderBy(s => s.Nombre)
                .Select(s => new { ID = s.ID, Texto = s.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/Etnias
        public JsonResult Etnias()
        {
            var items = _dbContext.tbl_Etnia
                .Where(e => e.IsActive)
                .OrderBy(e => e.Nombre)
                .Select(e => new { ID = e.ID, Texto = e.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/CondicionesSocioeconomicas
        public JsonResult CondicionesSocioeconomicas()
        {
            var items = _dbContext.tbl_CondicionSocioeconomica
                .Where(c => c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new { ID = c.ID, Texto = c.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/ViveCon
        public JsonResult ViveCon()
        {
            var items = _dbContext.tbl_ViveCon
                .Where(v => v.IsActive)
                .OrderBy(v => v.Nombre)
                .Select(v => new { ID = v.ID, Texto = v.Nombre })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: /Lookup/GetAniosEscolares
        public JsonResult GetAniosEscolares()
        {
            var items = _dbContext.tbl_AnioEscolar
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.Anio)
                .Select(a => new SelectItemViewModel { ID = a.ID, Texto = a.Anio.ToString() })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}