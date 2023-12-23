using Microsoft.AspNetCore.Mvc;

namespace Education.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignIn(int id)
        {
            return View();
        }
    }
}