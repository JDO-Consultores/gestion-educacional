using System.Web.Mvc;

namespace Gestionador.Controllers
{
    public class ErrorController : DefaultController
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}