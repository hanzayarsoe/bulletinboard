using Microsoft.AspNetCore.Mvc;

namespace MTM.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
