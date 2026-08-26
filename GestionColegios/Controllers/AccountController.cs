using GestionColegios.Interfaces;
using GestionColegios.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestionColegios.Controllers
{
    public class AccountController : DefaultController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AccountController(IAuthenticationService authenticationService, IJwtTokenService jwtTokenService, IUserService userService, IEmailService emailService)
        {
            _authenticationService = authenticationService;
            _jwtTokenService = jwtTokenService;
            _userService = userService;
            _emailService = emailService;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ResetPassword(string token)
        {
            return View(new ResetPasswordViewModel { Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _authenticationService.ValidateUserAsync(model.Username, model.Password);
                if (user != null)
                {
                    var token = _jwtTokenService.ValidateUser(user, user.Roles);
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7),
                    };

                    var authenticationManager = Request.GetOwinContext().Authentication;
                    authenticationManager.SignIn(authProperties, token);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", "Este usuario ha sido desactivado.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult _ChangePassword()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangePasswordAsync(PasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = await _userService.ChangePasswordAsync(UserId(), request), message = "Contraseña cambiada correctamente." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string baseUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}";

            if (await _userService.ForgotPasswordAsync(model, baseUrl))
            {
                ViewBag.Message = "Si el correo existe, recibirás un enlace de recuperación.";
            }
            else
            {
                ViewBag.Message = "Ha ocurrido un error.";
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _userService.ResetPasswordAsync(model);

            if (success)
            {
                ViewBag.Message = "Tu contraseña ha sido cambiada con éxito.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Message = "El token expiró o es invalido.";
                ModelState.AddModelError(model.Token, ViewBag.Message);
                return View(model);
            }
        }
    }
}