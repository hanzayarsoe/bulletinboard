using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MTM.Web.Controllers
{
	[Authorize]
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
			if (ModelState.IsValid)
			{
				ResponseModel response = _userService.EmailExists(email);
				if(response.ResponseType == Message.SUCCESS)
				{
                    var token = Guid.NewGuid().ToString();
                    var expiry = DateTime.UtcNow.AddHours(1);
                    resetTokens[email] = (token, expiry);
                    var resetLink = Url.Action(nameof(ResetPassword), "Account", new { email, token }, Request.Scheme);

                    HTMLMailData mailData = new HTMLMailData
                    {
                        EmailToId = "userid",
                        EmailToName = email,
                        EmailSubject = "Reset Password",
                        ResetLink = $"{resetLink}"
                    };

                    _mailService.SendHTMLMail(mailData);
                }
                AlertMessage(response);
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

            if (password == confirmPassword)
            {
                var email = model.email;
                var isValidPassword = IsPasswordValid(password);
                if (isValidPassword)
                {
                    ResponseModel idResponse = _userService.EmailExists(email);
                    if (idResponse.ResponseType == Message.SUCCESS)
                    {
                        string Id = idResponse.Data["Id"];
                        string hashedPassword = HashPassword(password);
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
                        AlertMessage(new ResponseModel { ResponseType = Message.FAILURE, ResponseMessage = "Email does not exist." });
                    }
                }
                else
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = "Password must be at least one lowercase letter, one uppercase letter, one digit, and one special character."
                    });
                }
            }
            else
            {
                AlertMessage(new ResponseModel
                {
                    ResponseType = Message.FAILURE,
                    ResponseMessage = "Passwords do not match."
                });
            }

            return View();
        }
        #endregion

        #region User Profile
		public IActionResult Profile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if(userId == null)
			{
				return RedirectToAction(nameof(Login));
			}

			var model = _userService.GetUser(userId);
			
			return View(model);
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
					ModelState.AddModelError("PasswordConfirm", "Password and confirmation password do not match.");
					return View(model);
				}
				model.PasswordHash = HashPassword(model.PasswordHash);
				model.Id = Guid.NewGuid().ToString();
				model.CreatedUserId = Guid.NewGuid().ToString();
				model.CreatedDate = DateTime.Now;
				ResponseModel response = _userService.Create(model);
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
				String PasswordHash = HashPassword(model.PasswordHash);
				ResponseModel response = _userService.Login(model.Email, PasswordHash);
				if(response.ResponseType == Message.SUCCESS)
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

		private static string HashPassword(string password)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(password);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("x2"));
				}
				return sb.ToString();
			}
		}

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            // Regex to enforce at least one lowercase letter, one uppercase letter, one digit, and one special character
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");

            return passwordRegex.IsMatch(password);
        }
        #endregion
    }
}
