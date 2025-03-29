using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;
using System.Diagnostics;

namespace RentFinder.Web.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRealEstateService _realEstateService;

    public HomeController(ILogger<HomeController> logger, IRealEstateService realEstateService)
    {
        _logger = logger;
        _realEstateService = realEstateService;
    }

    public async Task<IActionResult> Index()
    {
        var recommendedProperties = await _realEstateService.GetRecommendedPropertiesAsync();
        return View(recommendedProperties);
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