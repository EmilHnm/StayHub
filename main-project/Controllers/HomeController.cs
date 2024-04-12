using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using main_project.Models;
using Microsoft.EntityFrameworkCore;
using main_project.Service;

namespace main_project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["accommodationTypes"] = AccommodationTypeService.Instance().GetAccommodationTypes();
        ViewData["LatestHostels"] = HostelService.Instance().LatestHostels();
        return View();
    }

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
