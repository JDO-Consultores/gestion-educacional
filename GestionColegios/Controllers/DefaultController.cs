using GestionColegios.Filters;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    //[RateLimit(maxRequests: 10, seconds: 5)]
    public class DefaultController : Controller
    {
        public PartialViewResult _Utilities()
        {
            return PartialView();
        }

        public ActionResult _UrlAccionesAjax()
        {
            return PartialView();
        }

        internal int UserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return int.Parse(identity.Claims.SingleOrDefault(w => w.Type == ClaimTypes.Sid).Value);
        }

        internal string UserEmail()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity.Claims.SingleOrDefault(w => w.Type == ClaimTypes.Email).Value;
        }

        internal bool IsAdmin()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return roles.Contains("Administrador");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isAdmin = User.Identity.IsAuthenticated && User.IsInRole("Administrador");
            ViewBag.IsAdmin = isAdmin;
            base.OnActionExecuting(filterContext);
        }

        internal string GetCurrentUsername()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var nameClaim = identity?.Claims.SingleOrDefault(w => w.Type == ClaimTypes.Name);
            return nameClaim?.Value ?? UserEmail();
        }
    }
}