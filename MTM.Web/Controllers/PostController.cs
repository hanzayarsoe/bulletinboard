using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly IUserService _userService;
         public PostController(IPostService postService, IUserService userService)
        {
            this._postService = postService;
            this._userService = userService;
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
