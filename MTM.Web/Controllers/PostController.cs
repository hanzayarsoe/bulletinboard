using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTM.CommonLibrary;
using MTM.Entities.DTO;
using MTM.Services.IService;
using MTM.Services.Service;
using System.Diagnostics;
using System.Security.Claims;

namespace MTM.Web.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
         public PostController(IPostService postService)
        {
            this._postService = postService;
        }
        #region Post List
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

        #region GetPostList
        public IActionResult GetPostList()
        {
            string id = GetLoginId();
            PostViewModel model = _postService.GetPost(id);
            return View(model);
        }
        #endregion

        #region CreatePost
        public IActionResult CreatePost()
        {
            return View();
        }
        #endregion

        #endregion
        #region Edit
        public ActionResult Edit(string id)
        {
            String LoginRole = GetLoginRole();
            String LoginId = GetLoginId();
            if (id == null)
            {
                return NotFound();
            }

            PostViewModel post = _postService.GetPost(id);
            if (LoginRole != "admin" || LoginId != post.CreatedUserId )
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
            if (model == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                string LoginId = GetLoginId();
               
                model.CreatedDate = DateTime.Now;
                model.CreatedUserId = LoginId;
               // ResponseModel response = _postService
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
        public string GetLoginRole()
        {
            return HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? String.Empty;
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
