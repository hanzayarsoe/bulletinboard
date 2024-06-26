using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using MTM.Services.Service;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace MTM.Web.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        public PostController(IPostService postService, IUserService userService, IWebHostEnvironment env)
        {
            this._postService = postService;
            this._userService = userService;
            this._env = env; 
        }
        #region User List
        [HttpGet]
        public ActionResult Index()
        {
            if (TempData["MessageType"] != null)
            {                int ResponseType = Convert.ToInt32(TempData["MessageType"]);
                string ResponseMessage = Convert.ToString(TempData["Message"]) ?? string.Empty;
                AlertMessage(new ResponseModel
                {
                    ResponseType = ResponseType,
                    ResponseMessage = ResponseMessage
                });
            }
            return View();
        }
        #endregion
        #region Edit
        public ActionResult Edit(string id)
        {
            String LoginId = GetLoginId();
            UserViewModel loginUser = _userService.GetUser(LoginId);
            if (id == null)
            {
                return NotFound();
            }

            PostViewModel post = _postService.GetPost(id);
            if (loginUser.Role != 1 && LoginId != post.CreatedUserId )
            {
                TempData["MessageType"] = Message.FAILURE;
                TempData["Message"] = string.Format(Message.FAIL_AUTHORIZE, "Edit");
                return RedirectToAction("Index");
            }
            return View(post);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                string LoginId = GetLoginId();
                model.UpdatedDate = DateTime.Now;
                model.UpdatedUserId = LoginId;
                ResponseModel response = _postService.Update(model);
                TempData["MessageType"] = response.ResponseType;
                TempData["Message"] = response.ResponseMessage;
                return RedirectToAction("Index");
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
            var posts = new List<PostViewModel>();

            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "File is not selected" });
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                return Json(new { success = false, message = "Invalid file format" });
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
                        return Json(new { success = false, message = "Worksheet not found or the Excel file is empty." });
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        return Json(new { success = false, message = "The Excel file does not contain any data." });
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string Title = worksheet.Cells[row, 1].Text;
                        string Description = worksheet.Cells[row, 2].Text;
                      

                        if (string.IsNullOrEmpty(Title))
                        {
                            errorMessages.Add($"Row {row}: Title is required");
                            continue;
                        }
                        if (string.IsNullOrEmpty(Description))
                        {
                            errorMessages.Add($"Row {row}: Description is required");
                            continue;
                        }

                        var post = new PostViewModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            Title = Title,
                            Description = Description,
                            IsPublished = true,
                            CreatedDate = DateTime.Now,
                            CreatedUserId = GetLoginId()
                        };

                        posts.Add(post);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while processing the file. Details: {ex.Message}" });
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            foreach (var user in posts)
            {
               // var response = _userService.Register(user);
                //if (response.ResponseType != Message.SUCCESS)
                //{
                //    errorMessages.Add($"Error at row corresponding to {user.Email}: {response.ResponseMessage}");
                //}
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            return Json(new { success = true, message = "All users have been created successfully." });
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
                "Title", "Description", "Created Date"
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
