using Microsoft.AspNetCore.Mvc;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace MTM.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        #region GetAll
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        /// To get all Categories
        [HttpGet]
        public IActionResult GetAll()
        {
                UserListViewModel data;
                data = _userService.Data();
                return Json(data);
        }
        #endregion
    }
}
    