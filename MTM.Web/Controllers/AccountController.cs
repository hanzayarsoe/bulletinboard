using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;

namespace MTM.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            this._userService = userService;
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgetPassword(string email)
        {
            if (ModelState.IsValid)
            {
                ResponseModel response = _userService.EmailExists(email);
                AlertMessage(response);
            }

            return View();

        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        #region Common
        /// <summary>
        /// Alert message with viewData
        /// </summary>
        /// <param name="response"></param>
        private void AlertMessage(ResponseModel response)
        {
            ViewData["AlertMessage"] = response.ResponseMessage;
            switch (response.ResponseType)
            {
                case 1:
                    ViewData["AlertType"] = AlertType.Success.ToString().ToLower();
                    break;
                case 2:
                    ViewData["AlertType"] = AlertType.Error.ToString().ToLower();
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
