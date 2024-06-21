using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MTM.Web.Controllers
{

    public class AccountController : Controller
	{
		private readonly IUserService _userService;
		private readonly IMailService _mailService;
        static Dictionary<string, (string token, DateTime expiry)> resetTokens = new Dictionary<string, (string token, DateTime expiry)>();

        public AccountController(IUserService userService, IMailService mailService)
		{
			this._userService = userService;
			this._mailService = mailService;
		}
		#region Auth/Forget Password
		public IActionResult ForgetPassword()
		{
            if (User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ForgetPassword(string email)
		{
			var response = new ResponseModel();
			if (ModelState.IsValid)
			{
				bool isExist = _userService.CheckEmail(email);
				if(isExist)
				{
                    var token = Guid.NewGuid().ToString();
                    var expiry = DateTime.UtcNow.AddHours(1);
                    resetTokens[email] = (token, expiry);
                    var resetLink = Url.Action(nameof(ResetPassword), "Account", new { email, token }, Request.Scheme);

					HTMLMailData mailData = new HTMLMailData
					{
						EmailToId = "anonymous",
						EmailToName = email,
						EmailSubject = "Reset Password",
						ResetLink = $"{resetLink}"
					};
					_mailService.SendHTMLMail(mailData);
					response.ResponseType = Message.SUCCESS;
					response.ResponseMessage = string.Format(Message.SAVE_SUCCESS, "Email", "sent to " + email);
					AlertMessage(response);
                }
				else
				{
                    response.ResponseType = Message.FAILURE;
                    response.ResponseMessage = string.Format(Message.NOT_EXIST, "Your email");
                    AlertMessage(response);
                }
			}

			return View();

		}
        #endregion

        #region Auth/ResetPassword
		public IActionResult ResetPassword(string email,string token)
		{
            if (User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction(nameof(Login));
            }

            if (resetTokens.ContainsKey(email))
            {
                var storedToken = resetTokens[email].token;
                var expiryTime = resetTokens[email].expiry;

                if (storedToken == token && expiryTime > DateTime.UtcNow)
                {
                    ResetPasswordModel model = new ResetPasswordModel
                    {
                        email = email,
                        token = token
                    };
                    return View(model);
                }
                else
                {
                    Debug.WriteLine("Token mismatch or token has expired.");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction(nameof(ForgetPassword));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            UserViewModel user = new UserViewModel();
            var password = model.password;
            var confirmPassword = model.confirmPassword;
            var email = model.email;

			if(email == null || password == null || confirmPassword == null)
			{
				return NotFound();
			}

            if (password == confirmPassword)
            {
                var isValidPassword = Helpers.IsPasswordValid(password);
                if (isValidPassword)
                {
                    ResponseModel idResponse = _userService.GetIdByEmail(email);
                    if (idResponse.ResponseType == Message.SUCCESS)
                    {
                        string Id = idResponse.Data["Id"];
                        string hashedPassword = Helpers.HashPassword(password);
                        user.Id = Id;
                        user.PasswordHash = hashedPassword;
                        ResponseModel response = _userService.Update(user);
						if(response.ResponseType == Message.SUCCESS)
						{
							return RedirectToAction(nameof(Login));
						}
                        AlertMessage(response);
                    }
                    else
                    {
						AlertMessage(new ResponseModel { ResponseType = Message.FAILURE, ResponseMessage = string.Format(Message.NOT_EXIST,email) });
                    }
                }
                else
                {
					AlertMessage(new ResponseModel
					{
						ResponseType = Message.FAILURE,
						ResponseMessage = Message.PASSWORD_FORMAT_ERROR
					});
                }
            }
            else
            {
                AlertMessage(new ResponseModel
                {
                    ResponseType = Message.FAILURE,
                    ResponseMessage = string.Format(Message.NOT_MATCH,"Your Password")
                });
            }

            return View();
        }
        #endregion

        #region Create
        [HttpGet]
		public IActionResult Register()
		{
            if (User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.PasswordHash != model.PasswordConfirm)
				{
					AlertMessage(new ResponseModel
					{
						ResponseType = Message.FAILURE,
						ResponseMessage = String.Format(Message.NOT_MATCH, model.PasswordHash)
					});
					return View(model);
				}
                if (!Helpers.IsPasswordValid(model.PasswordHash))
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = Message.PASSWORD_FORMAT_ERROR
                    });
                    return View(model);
                }
				model.Id = Guid.NewGuid().ToString();
				model.CreatedUserId = Guid.NewGuid().ToString();
				model.CreatedDate = DateTime.Now;
				ResponseModel response = _userService.Register(model);
				if(response.ResponseType == Message.SUCCESS)
				{
					return RedirectToAction("Login");
				}
				AlertMessage(response);
			}
			return View(model);
		}
		#endregion

		#region Login And Logout
		[HttpGet]
		public IActionResult Login()
		{
            if (User.Identity != null && User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				ResponseModel response = new ResponseModel();
				response = _userService.Login(model.Email, model.PasswordHash);
				
                if (response.ResponseType == Message.SUCCESS)
				{
					string Id = response.Data["Id"];
					string Email = response.Data["Email"];
					List<Claim> Claims = new List<Claim>
					{
					new Claim(ClaimTypes.Name, Email),
					new Claim(ClaimTypes.NameIdentifier, Id),
					};
					var ClaimsIdentity = new ClaimsIdentity(Claims, "CookieAuth");
					var AuthProperties = new AuthenticationProperties
					{
						IsPersistent = true,
						ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
					};
					await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(ClaimsIdentity), AuthProperties);
					TempData["FullName"] = response.Data["FullName"];
					return RedirectToAction("Index", "Home");
				}
				AlertMessage(response);
			}
			return View(model);
		}

		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CookieAuth");
			return RedirectToAction("Login", "Account");
		}
		#endregion

		#region Common
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
