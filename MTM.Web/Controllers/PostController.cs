﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        #region Post List
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

        #region GetPostList
        [HttpGet]
        public IActionResult GetPostList()
        {
            PostListViewModel postList = _postService.GetPostList();
            return Json(postList);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PostViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid().ToString();
                model.CreatedUserId = GetLoginId();
                model.CreatedDate = DateTime.Now;
                ResponseModel response = _postService.Create(model);
                AlertMessage(response);
                if (response.ResponseType == Message.SUCCESS)
                {
                    TempData["MessageType"] = Message.SUCCESS;
                    TempData["Message"] = string.Format(Message.SAVE_SUCCESS, "Post", "Created");
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        #endregion

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
                return Json(new { success = false, message = Message.NOT_SELECTED });
            }

            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
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
                        return Json(new { success = false, message = string.Format(Message.NOT_FOUND, "WorkSheet") });
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount < 2)
                    {
                        return Json(new { success = false, message = string.Format(Message.NOT_FOUND, "No data") });
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string Title = worksheet.Cells[row, 1].Text;
                        string Description = worksheet.Cells[row, 2].Text;

                        if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description))
                        {
                            errorMessages.Add(string.Format(Message.REQUIRED, row));
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
                return Json(new { success = false, message = string.Format(Message.ERROR_OCCURED, ex.Message) });
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            foreach (var post in posts)
            {
                var response = _postService.Create(post);
            }

            if (errorMessages.Any())
            {
                var errorMessageHtml = $"<ul style='font-size:small; list-style-type:none;'>{string.Join("", errorMessages.Select(e => $"<li>{e}</li>"))}</ul>";
                return Json(new { success = false, message = errorMessageHtml, errors = errorMessages });
            }

            return Json(new { success = true, message = string.Format(Message.CREATE_SUCCESS, "All Posts") });
        }
        #endregion

        #region Export
        public IActionResult Export()
        {
            var posts = _postService.GetPostList().PostList;
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                var headers = new string[]
                {
                "Title", "Description"
                };

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[1, col].Value = headers[col - 1];
                }

                int row = 2;
                foreach (var post in posts)
                {
                    worksheet.Cells[row, 1].Value = post.Title;
                    worksheet.Cells[row, 2].Value = post.Description;
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
