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
                string OldPassword = model.oldPassword ?? string.Empty;
                string NewPassword = model.password ?? string.Empty;
                string ConfirmPassword = model.confirmPassword ?? string.Empty;
               
                if(NewPassword != ConfirmPassword)
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = string.Format(Message.NOT_MATCH, "Password")
                    });
                    return View(model);
                }

                if (!Helpers.IsPasswordValid(NewPassword))
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = Message.PASSWORD_FORMAT_ERROR
                    });
                    return View(model);
                }
 
                ResponseModel response = this._userService.UpdatePassword(LoginUserId, OldPassword, NewPassword);
                AlertMessage(response);
                return View(model);
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
    