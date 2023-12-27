using Microsoft.AspNetCore.Mvc;

namespace Education.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            /*  // Check if the user is logged in
              if (HttpContext.Session.GetString("user") == null)
              {
                  // User is not logged in, redirect to the sign-in page
                  return RedirectToAction("signin", "auth", new { area = "admin" });
              }*/

            // User is logged in, continue to the home page
            return View();
        }
    }
}
