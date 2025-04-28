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

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] PropertySearchModel searchModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var results = await _realEstateService.SearchPropertiesAsync(searchModel);
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _realEstateService.GetPropertyDetailAsync(id);
        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommended()
    {
        var recommendedProperties = await _realEstateService.GetRecommendedPropertiesAsync();
        return Ok(recommendedProperties);
    }
}
