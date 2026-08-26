using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ReglamentoInterno()
        {
            ViewBag.PdfPath = Url.Content("~/Content/Assets/Reglamento/ReglamentoInterno.pdf");
            return View();
        }

        public ActionResult Ayuda()
        {
            return View();
        }
    }
}