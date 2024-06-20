using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.Data;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
		public IActionResult ResetPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ResetPassword(string pw1,string pw2)
		{
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
					TempData["FullName"] = response.Data["FullName"]; // Temporary Code To Delete
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
		#endregion
	}
}
