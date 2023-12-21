using Microsoft.AspNetCore.Mvc;

namespace Education.Controllers
{
    public class NotFoundController : Controller
    {
        private readonly ILogger<NotFoundController> _logger;

        public NotFoundController(ILogger<NotFoundController> logger)
        {
            _logger = logger;
        }

        [Route("/error/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            return View();
        }
    }
}
