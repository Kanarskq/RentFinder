using Microsoft.AspNetCore.Mvc;
using RentFinder.Web.Mvc.Models;
using RentFinder.Web.Mvc.Services;

namespace RentFinder.Web.Mvc.Controllers;

public class PropertiesController : Controller
{
    private readonly IRealEstateService _realEstateService;

    public PropertiesController(IRealEstateService realEstateService)
    {
        _realEstateService = realEstateService;
    }

    public IActionResult Search()
    {
        return View(new PropertySearchModel());
    }

    [HttpPost]
    public async Task<IActionResult> Search(PropertySearchModel searchModel)
    {
        if (!ModelState.IsValid)
        {
            return View(searchModel);
        }

        var results = await _realEstateService.SearchPropertiesAsync(searchModel);
        return View("SearchResults", results);
    }

    public async Task<IActionResult> Details(int id)
    {
        var property = await _realEstateService.GetPropertyDetailAsync(id);
        if (property == null)
        {
            return NotFound();
        }

        return View(property);
    }
}
