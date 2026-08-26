using Gestionador.Interfaces;
using Gestionador.Models;
using Gestionador.Responses;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gestionador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsersController : DefaultController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Usuarios()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> _DeleteUser(int id)
        {
            var user = await GetUserByID(id);
            return PartialView(user);
        }

        [HttpGet]
        public async Task<ActionResult> _CreateUser()
        {
            var model = new UserCreateViewModel
            {
                IsActive = true,
                IsAdmin = false
            };
            return PartialView(model);
        }

        [HttpGet]
        public async Task<ActionResult> _EditUser(int id)
        {
            var user = await GetUserByID(id);
            return PartialView(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateUserAsync(UserCreateViewModel user)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _userService.CreateUserAsync(UserId(), user);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -2)
            {
                return Json(new { success = false, message = "El usuario ya existe." }, JsonRequestBehavior.AllowGet);
            }
            else if (result == -1)
            {
                return Json(new { success = false, message = "Las contraseñas no coinciden." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditUserAsync(UserCreateViewModel user)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values.SelectMany(v => v.Errors).Where(e => !string.IsNullOrEmpty(e.ErrorMessage)).Select(e => e.ErrorMessage).FirstOrDefault();
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            var result = await _userService.UpsertUserAsync(UserId(), user);

            if (result > 0)
            {
                return Json(new { success = true, message = "Cambios Guardados Correctamente." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Operación fallida." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<UserResponse> GetUserByID(int id)
        {
            return await _userService.GetUserByID(id);
        }

        [HttpGet]
        public async Task<UserResponse> GetUserByUsername()
        {
            return await _userService.GetUserByUsername(UserEmail());
        }

        [HttpGet]
        public async Task<JsonResult> GetUsers()
        {
            dynamic usuarios = await _userService.GetUsers();
            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRoles()
        {
            dynamic roles = _userService.GetRoles();
            return Json(roles, JsonRequestBehavior.AllowGet);
        }
    }
}