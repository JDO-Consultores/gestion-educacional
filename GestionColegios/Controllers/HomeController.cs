using GestionColegios.Interfaces;
using GestionColegios.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    [Authorize]
    public class HomeController : DefaultController
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        // GET: / (página de bienvenida pública)
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Dashboard");
            return View();
        }

        // GET: /Home/Dashboard
        public async Task<ActionResult> Dashboard()
        {
            var vm = await _homeService.GetDashboardStatsAsync();
            return View(vm);
        }

        // ── Ayuda en línea ────────────────────────────────────────────────
        // Catálogo de temas de ayuda (título visible -> nombre de archivo .md
        // dentro de la carpeta /docs). El acceso por nombre se valida contra
        // esta lista para evitar path traversal.
        private static readonly List<(string Grupo, string Titulo, string Archivo)> _temasAyuda
            = new List<(string, string, string)>
        {
            // Matrícula y ciclo del alumno
            ("Matrícula y ciclo del alumno", "Proceso general de matrícula",      "PROCESO_MATRICULA.md"),
            ("Matrícula y ciclo del alumno", "Alumno retirado que reingresa",     "PROCESO_MATRICULA_REINGRESO.md"),
            ("Matrícula y ciclo del alumno", "Cierre de año (promoción)",         "PROCESO_PROMOCION_ANIO.md"),
            ("Matrícula y ciclo del alumno", "Lista de espera (cupos)",           "PROCESO_LISTA_ESPERA.md"),
            ("Matrícula y ciclo del alumno", "Alumno Nuevo / Alumno Antiguo",     "FLAG_ALUMNO_NUEVO_ANTIGUO.md"),

            // Ficha del alumno
            ("Ficha del alumno", "Ficha del alumno",                              "PROCESO_FICHA_ALUMNO.md"),
            ("Ficha del alumno", "Retiro del alumno",                             "PROCESO_RETIRO_ALUMNO.md"),
            ("Ficha del alumno", "Cambio de RUT (traspaso)",                      "PROCESO_CAMBIO_RUT.md"),
            ("Ficha del alumno", "Alergias y discapacidades",                     "PROCESO_ALERGIAS_DISCAPACIDADES.md"),
            ("Ficha del alumno", "Apoderados",                                    "PROCESO_APODERADOS.md"),
            ("Ficha del alumno", "Documentos del alumno",                         "PROCESO_DOCUMENTOS.md"),
            ("Ficha del alumno", "Certificados del alumno",                       "PROCESO_CERTIFICADOS.md"),

            // Configuración académica
            ("Configuración académica", "Año Escolar y Cursos",                   "PROCESO_ANIO_ESCOLAR_CURSOS.md"),
            ("Configuración académica", "Profesores Jefe",                        "PROCESO_PROFESORES_JEFE.md"),

            // Plataforma
            ("Plataforma", "Autenticación y usuarios",                            "PROCESO_AUTENTICACION_USUARIOS.md"),
            ("Plataforma", "Panel de inicio (Dashboard)",                         "PROCESO_DASHBOARD.md"),
            ("Plataforma", "Historial / Log de actividad",                        "PROCESO_HISTORIAL_LOG.md"),
        };

        [AllowAnonymous]
        public ActionResult Ayuda(string topic = null)
        {
            var temas = _temasAyuda
                .GroupBy(t => t.Grupo)
                .Select(g => new AyudaGrupoViewModel
                {
                    Grupo = g.Key,
                    Items = g.Select(t => new AyudaTemaViewModel
                    {
                        Titulo  = t.Titulo,
                        Archivo = t.Archivo
                    }).ToList()
                })
                .ToList();

            ViewBag.Temas = temas;
            ViewBag.TopicInicial = !string.IsNullOrEmpty(topic)
                ? topic
                : _temasAyuda.First().Archivo;
            return View();
        }

        // GET: /Home/AyudaContenido?topic=PROCESO_MATRICULA.md
        // Devuelve el contenido del .md solicitado (texto plano) si está
        // declarado en el catálogo de temas. Cualquier otro nombre es rechazado.
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AyudaContenido(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic) ||
                !_temasAyuda.Any(t => t.Archivo == topic))
                return HttpNotFound();

            var fullPath = Server.MapPath("~/docs/" + topic);
            if (!System.IO.File.Exists(fullPath))
                return HttpNotFound();

            // Detectar encoding del archivo (los .md fueron creados con codificación
            // Windows-1252 por la herramienta original). Se valida si los bytes son
            // UTF-8 válidos SIN lanzar excepciones (evita romper la depuración) y, en
            // caso contrario, se interpretan como Windows-1252 para no mostrar
            // caracteres rotos.
            var raw = System.IO.File.ReadAllBytes(fullPath);
            var markdown = EsUtf8Valido(raw)
                ? new System.Text.UTF8Encoding(false).GetString(raw)
                : System.Text.Encoding.GetEncoding(1252).GetString(raw);

            return Content(markdown, "text/plain; charset=utf-8", System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Determina si la secuencia de bytes es UTF-8 válida sin lanzar excepciones.
        /// Se usa para elegir entre UTF-8 y Windows-1252 al leer los archivos de ayuda.
        /// </summary>
        private static bool EsUtf8Valido(byte[] bytes)
        {
            int i = 0;
            int n = bytes.Length;

            // Saltar BOM UTF-8 si está presente.
            if (n >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                i = 3;

            while (i < n)
            {
                byte b = bytes[i];

                if (b <= 0x7F) // ASCII (0xxxxxxx)
                {
                    i++;
                    continue;
                }

                int extra;       // bytes de continuación esperados
                int min;         // valor mínimo del code point (detecta overlong)
                int codepoint;

                if ((b & 0xE0) == 0xC0) { extra = 1; min = 0x80;    codepoint = b & 0x1F; }
                else if ((b & 0xF0) == 0xE0) { extra = 2; min = 0x800;   codepoint = b & 0x0F; }
                else if ((b & 0xF8) == 0xF0) { extra = 3; min = 0x10000; codepoint = b & 0x07; }
                else return false; // byte líder inválido

                if (i + extra >= n) return false; // faltan bytes de continuación

                for (int j = 1; j <= extra; j++)
                {
                    byte cont = bytes[i + j];
                    if ((cont & 0xC0) != 0x80) return false; // debe ser 10xxxxxx
                    codepoint = (codepoint << 6) | (cont & 0x3F);
                }

                if (codepoint < min) return false;                            // overlong
                if (codepoint > 0x10FFFF) return false;                        // fuera de rango
                if (codepoint >= 0xD800 && codepoint <= 0xDFFF) return false;  // surrogates

                i += extra + 1;
            }

            return true;
        }

        // GET: /Home/GetAlumnosPorCurso?anioEscolarId=3
        [HttpGet]
        public async Task<JsonResult> GetAlumnosPorCurso(int anioEscolarId)
        {
            var data = await _homeService.GetAlumnosPorCursoAsync(anioEscolarId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: /Home/GetActividadReciente
        [HttpGet]
        public async Task<JsonResult> GetActividadReciente()
        {
            var data = await _homeService.GetActividadRecienteAsync(15);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}