using Microsoft.AspNetCore.Mvc;

namespace Education.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}