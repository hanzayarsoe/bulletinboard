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
        private readonly IWebHostEnvironment _env;

		public UserController(IUserService userService, IWebHostEnvironment env)
        {
			this._userService = userService;
            this._env = env;
		}

        #region User List
        [HttpGet]
        public ActionResult Index()
        {
            string Id = GetLoginId();
            UserListViewModel model = _userService.GetList(Id);
            if (TempData["MessageType"] != null)
            {
                int ResponseType = Convert.ToInt32(TempData["MessageType"]);
                string ResponseMessage = Convert.ToString(TempData["Message"]) ?? string.Empty;
                AlertMessage(new ResponseModel
                {
                    ResponseType = ResponseType,
                    ResponseMessage = ResponseMessage
                });
            }
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
                model.UpdatedUserId = model.Id;
                ResponseModel response = _userService.Update(model);
                AlertMessage(response);
            }

            return View(model);
        }
        #endregion

        #region User Detail
  
        public IActionResult UserDetail(string Id)
        {
            UserViewModel user = _userService.GetUser(Id);
            if (user != null)
            {
                return Json(user); 
            }
            return NotFound(); 
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

        #region Create
        public IActionResult Create(string id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserViewModel model)
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
                model.CreatedUserId = GetLoginId();
                model.CreatedDate = DateTime.Now;
                ResponseModel response = _userService.Register(model);
                AlertMessage(response);
                if(response.ResponseType == Message.SUCCESS)
                {
                    TempData["MessageType"] = Message.SUCCESS;
                    TempData["Message"] = string.Format(Message.SAVE_SUCCESS, "User", "Created");
                    return RedirectToAction("Index");
                }
              
            }
            return View(model);
        }
        #endregion

        #region Delete
        public IActionResult Delete(string id)
        {
            string LoginId = GetLoginId();
            ResponseModel response = _userService.Delete(id, LoginId);
            return Json(response);
        }
        #endregion

        #region Import User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Import()
        {
            return View();
        }
        #endregion

        #region Edit
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserViewModel user = _userService.GetUser(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserViewModel model)
        {
            if(model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                model.UpdatedUserId = GetLoginId();
                ResponseModel response = _userService.Update(model);
                AlertMessage(response);
            }
            return View(model);
        }
        #endregion

        #region
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "File is not Selected" });
            }

            var fileExtension = Path.GetExtension(file.FileName);

            if (fileExtension != ".csv" && fileExtension != ".xlsx")
            {
                return Json(new { success = false, message = "Invalid File Format" });
            }

            var uploads = Path.Combine(_env.WebRootPath, "Uploads");

            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, file.FileName);

            if (System.IO.File.Exists(filePath))
            {
                filePath = GetUniqueFileName(filePath, uploads);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            ResponseModel response = _userService.UploadUser(filePath);
            if (response.ResponseType == Message.SUCCESS)
            {
                return Json(new { success = true, message = response.ResponseMessage });
            }
            else
            {
                return Json(new { success = false, message = response.ResponseMessage });
            }
        }

        private string GetUniqueFileName(string filePath, string uploads)
        {
            int count = 1;
            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string newFullPath = filePath;

            while (System.IO.File.Exists(newFullPath))
            {
                string tempFileName = $"{fileNameOnly} ({count++})";
                newFullPath = Path.Combine(uploads, tempFileName + extension);
            }

            return newFullPath;
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
    