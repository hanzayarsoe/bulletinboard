using Microsoft.AspNetCore.Mvc;
using MTM.Entities.DTO;
using MTM.Services.IService;
using System.Diagnostics;
using System.Security.Claims;

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
            string Id = GetLoginId();    
			UserListViewModel model;
            model = _userService.GetList();
            Debug.WriteLine("LOgin User Id-----------------" + Id);
            return View(model.UserList);
        }
		#endregion

		#region Common
        public string GetLoginId()
        {
            return HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
		}
		#endregion

	}
}
    