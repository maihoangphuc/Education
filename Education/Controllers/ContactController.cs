using Education.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Education.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
