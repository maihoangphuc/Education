using Education.Models;
using Microsoft.AspNetCore.Mvc;

namespace Education.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private static readonly string Username = "admin";
        private static readonly string Password = "12345678";

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(AccountModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username.ToLower() == Username && model.Password.ToLower() == Password)
                {
                    HttpContext.Session.SetString("user", "admin");
                    return RedirectToAction("index", "home", new { area = "admin" });
                }
                else
                {
                    // Nếu thông tin đăng nhập không đúng, thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError("Username", "Tên người dùng hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("signin", "auth", new { area = "admin" });
        }
    }
}
