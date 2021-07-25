using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BugTracker.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        // PROPERTIES
        private readonly ILogger<HomeController> _logger;




        // CONSTRUCTORS
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }




        // ACTIONS / METHODS
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
