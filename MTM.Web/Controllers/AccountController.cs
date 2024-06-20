using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Security.Cryptography;
using System.Text;

namespace MTM.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IUserService _userService;

		public AccountController(IUserService userService)
		{
			this._userService = userService;
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
				AlertMessage(response);
			}

			return View();

		}
		#endregion

		#region Create
		[HttpGet]
		public IActionResult Register()
		{
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
				AlertMessage(response);
			}
			return View(model);
		}
		#endregion

		#region Login
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(UserViewModel model)
		{
			//if (ModelState.IsValid)
			//{
			//	model.Id = Guid.NewGuid().ToString();
			//	model.CreatedUserId = Guid.NewGuid().ToString();
			//	model.CreatedDate = DateTime.Now;
			//	ResponseModel response = _userService.Create(model);
			//	AlertMessage(response);
			//}
			return View(model);
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
					ViewData["AlertType"] = AlertType.Error.ToString().ToLower();
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
				// Convert the input string to a byte array and compute the hash.
				byte[] inputBytes = Encoding.UTF8.GetBytes(password);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to a hexadecimal string.
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
