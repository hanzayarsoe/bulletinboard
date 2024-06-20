
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MTM.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var FullName = TempData["FullName"] as string ?? string.Empty; // Temporary Code To Delete
            ViewData["FullName"] = FullName; // Temporary Code To Delete
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
