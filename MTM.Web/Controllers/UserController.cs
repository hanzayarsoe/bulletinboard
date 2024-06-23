using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Diagnostics;
using System.Security.Claims;

namespace MTM.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
		public UserController(IUserService userService)
        {
			this._userService = userService;
		}

        #region User List
        [HttpGet]
        public ActionResult Index()
        {
            string Id = GetLoginId();
            UserListViewModel model;
            model = _userService.GetList(Id);
            return View(model.UserList);
        }
        #endregion

        #region UserProfile
        public IActionResult UserProfile()
        {
            var userId = GetLoginId();
            UserViewModel user = _userService.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserProfile(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = GetLoginId();
                ResponseModel response = _userService.Update(model);
                AlertMessage(response);
            }

            return View(model);
        }
        #endregion

        #region Change Password
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string LoginUserId = GetLoginId();
                UserViewModel user = new UserViewModel();
                string oldpassword = model.oldPassword ?? string.Empty;
                string password = model.oldPassword ?? string.Empty;
                string confirmPassword = model.oldPassword ?? string.Empty;
                if (string.IsNullOrEmpty(oldpassword) || string.IsNullOrEmpty(password))
                {
                    View(model);
                }
                // if (password != !)
                user.Id = LoginUserId;
                user.PasswordHash = password;
                user.UpdatedDate = DateTime.Now;
                user.UpdatedUserId = LoginUserId;
                ResponseModel response = this._userService.Update(user);
                return RedirectToAction("Index", "Account");
            }
            return View(model);
        }
        #endregion

        #region Common
        public string GetLoginId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
        }

        private void AlertMessage(ResponseModel response)
        {
            ViewData["AlertMessage"] = response.ResponseMessage;
            switch (response.ResponseType)
            {
                case 1:
                    ViewData["AlertType"] = AlertType.Success.ToString().ToLower();
                    break;
                case 2:
                    ViewData["AlertType"] = AlertType.Danger.ToString().ToLower();
                    break;
                case 3:
                    ViewData["AlertType"] = AlertType.Warning.ToString().ToLower();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
    