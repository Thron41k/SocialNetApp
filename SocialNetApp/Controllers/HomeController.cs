using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SocialNetApp.Models;
using SocialNetApp.ViewModels.Account;

namespace SocialNetApp.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    [Route("")]
    [Route("[controller]/[action]")]
    public IActionResult Index()
    {
        return View(new MainViewModel());
    }

    [Route("[action]")]
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