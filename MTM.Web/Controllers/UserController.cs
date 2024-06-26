using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using OfficeOpenXml;
using System.Globalization;
using System.Security.Claims;

namespace MTM.Web.Controllers
{
    [Authorize]
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
            return View();
        }

        public ActionResult GetList()
        {
            string Id = GetLoginId();
            UserListViewModel model = _userService.GetList(Id);
            return Json(model);

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
                var isExist = _userService.CheckEmail(model.Email);
                ResponseModel response = _userService.GetIdByEmail(model.Email);
                string? emailId = response.Data != null && response.Data.ContainsKey("Id") ? response.Data["Id"] : null;
                if ((isExist && model.Id == emailId) || !isExist)
                {
                    ResponseModel updateInfo = _userService.Update(model);
                    AlertMessage(updateInfo);
                }
                else
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = Message.EMAIL_FAIL
                    });
                    return View(model);
                }
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
                var currentUserId = GetLoginId();
                model.UpdatedUserId = currentUserId;
                var isExist = _userService.CheckEmail(model.Email);
                ResponseModel response = _userService.GetIdByEmail(model.Email);
                string? emailId = response.Data != null && response.Data.ContainsKey("Id") ? response.Data["Id"] : null;
                if ((isExist && emailId == model.Id))
                {
                    ResponseModel updateInfo = _userService.Update(model);
                    AlertMessage(updateInfo);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AlertMessage(new ResponseModel
                    {
                        ResponseType = Message.FAILURE,
                        ResponseMessage = Message.EMAIL_FAIL
                    });
                    return View(model);
                }
                
            }
            return View(model);
        }
        #endregion

        #region Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile file)
        {
            var errorMessages = new List<string>();
            var users = new List<UserViewModel>();

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = Message.NOT_SELECTED });
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".csv" && fileExtension != ".xlsx")
            {
                return Json(new { success = false, message = Message.INVALID_FORMAT });
            }

            var uploads = Path.Combine(_env.WebRootPath, "Uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, file.FileName);
            if (System.IO.File.Exists(filePath))
            {
                filePath = Helpers.GetUniqueFileName(filePath, uploads);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = string.Format(Message.NOT_FOUND,"WorkSheet") });
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        return Json(new { success = false, message = string.Format(Message.NOT_FOUND,"No data") });
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var firstName = worksheet.Cells[row, 1].Text;
                        var lastName = worksheet.Cells[row, 2].Text;
                        var email = worksheet.Cells[row, 3].Text;
                        var phone = worksheet.Cells[row, 4].Text;
                        var password = worksheet.Cells[row, 5].Text;
                        var confirmPassword = worksheet.Cells[row, 6].Text;
                        var roleName = worksheet.Cells[row, 7].Text;
                        var dobString = worksheet.Cells[row, 8].Text;
                        var address = worksheet.Cells[row, 9].Text;

                        if (string.IsNullOrEmpty(firstName))
                        {
                            errorMessages.Add(string.Format(Message.REQUIRED_NAME,row));
                            continue;
                        }

                        var isEmailExist = _userService.CheckEmail(email);
                        if (isEmailExist)
                        {
                            errorMessages.Add(string.Format(Message.EMAIL_EXIST,row,email));
                            continue;
                        }

                        if (password != confirmPassword)
                        {
                            errorMessages.Add(string.Format(Message.NOT_MATCH,"Row " + row + " your password"));
                            continue;
                        }

                        DateTime? dob = null;
                        if (!string.IsNullOrEmpty(dobString))
                        {
                            if (!DateTime.TryParseExact(dobString, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDob))
                            {
                                errorMessages.Add(string.Format(Message.DATE_ERROR,row));
                                continue;
                            }
                            dob = parsedDob;
                        }

                        int? role = Helpers.GetRoleValue(roleName);
                        var user = new UserViewModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            FirstName = firstName,
                            LastName = lastName,
                            Email = email,
                            PhoneNumber = phone,
                            PasswordHash = password,
                            Role = role,
                            DOB = dob,
                            Address = address,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = GetLoginId()
                        };

                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = string.Format(Message.ERROR_OCCURED,ex.Message)});
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            foreach (var user in users)
            {
                var response = _userService.Register(user);
                if (response.ResponseType != Message.SUCCESS)
                {
                    errorMessages.Add(string.Format(Message.CORRESPONSE_ERROR, user.Email, response.ResponseMessage));
                }
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            return Json(new { success = true, message = string.Format(Message.CREATE_SUCCESS,"All users")});
        }
        #endregion

        #region Export
        public IActionResult Export()
        {
            var users = _userService.GetUserListData().UserList;
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                var headers = new string[]
                {
                "First Name", "Last Name", "Email", "Phone Number", "Address", "Created Date"
                };

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[1, col].Value = headers[col - 1];
                }

                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = user.FirstName;
                    worksheet.Cells[row, 2].Value = user.LastName;
                    worksheet.Cells[row, 3].Value = user.Email;
                    worksheet.Cells[row, 4].Value = user.PhoneNumber;
                    worksheet.Cells[row, 5].Value = user.Address;
                    worksheet.Cells[row, 6].Value = user.CreatedDate.ToString("yyyy-MM-dd");

                    row++;
                }

                package.Save();
            }
            stream.Position = 0;
            var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, mimeType);
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
    