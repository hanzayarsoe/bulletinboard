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

        #region User List
        [HttpGet]
        public ActionResult Index()
        {
            UserListViewModel model;
            model = _userService.GetList();
            return View(model.UserList);
        }
        #endregion
    }
}
    