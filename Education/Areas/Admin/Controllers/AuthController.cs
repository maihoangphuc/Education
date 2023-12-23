using Education.Models;
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
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username.ToLower() == "admin" && model.Password.ToLower() == "12345678")
                {
                    HttpContext.Session.SetString("user", "admin");
                    return RedirectToAction("index", "home", new { area = "admin" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tên người dùng hoặc mật khẩu không hợp lệ");
                }
            }

            return View(model);
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("signin");
        }
    }
}
