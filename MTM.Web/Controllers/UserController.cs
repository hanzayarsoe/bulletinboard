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
                return RedirectToAction("index", "Category");
            }
            return View(model);
        }
        #endregion

        #region Common
        public string GetLoginId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
		}
		#endregion

	}
}
    