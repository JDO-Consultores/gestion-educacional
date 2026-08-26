using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: /Error  (defaultRedirect)
        public ActionResult Index()
        {
            return InternalServer();
        }

        // GET: /Error/InternalServer  (HTTP 500)
        public ActionResult InternalServer()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View("InternalServer");
        }

        // GET: /Error/NotFound  (HTTP 404)
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("NotFound");
        }
    }
}
