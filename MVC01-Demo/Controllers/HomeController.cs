using System.Diagnostics;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MVC01_Demo.Models;

namespace MVC01_Demo.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
            => View(await _analyticsService.GetAnalyticsDataAsync(ct));


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
