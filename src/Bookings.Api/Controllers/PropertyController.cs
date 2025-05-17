using Bookings.Api.Controllers.Request.Properties;
using Bookings.Application.Commands.Properties;
using Bookings.Application.Queries.Properties;
using Bookings.Application.Services.Properties;
using Bookings.Application.Services.Search;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Controllers;
[ApiController]
[Route("api/property")]
public class PropertyController : ControllerBase
{
    private readonly ILogger<PropertyController> _logger;
    private readonly IMediator _mediator;
    private readonly PropertyServices _propertyServices;
    private readonly IPropertyQueries _queries;
    private readonly IPropertySearchEngine _searchEngine;

    public PropertyController(
        ILogger<PropertyController> logger,
        IMediator mediator,
        PropertyServices propertyServices,
        IPropertyQueries queries,
        PropertySearchEngine searchEngine)
    {
        _logger = logger;
        _mediator = mediator;
        _queries = queries;
        _searchEngine = searchEngine;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request)
    {
        _logger.LogInformation("Creating property for OwnerId: {OwnerId}", request.OwnerId);

        var command = new CreatePropertyCommand(
            request.OwnerId,
            request.Title,
            request.Description,
            request.Latitude,
            request.Longitude,
            request.Price,
            request.Bedrooms,
            request.Bathrooms,
            request.SquareFootage,
            request.HasBalcony,
            request.HasParking,
            request.PetsAllowed,
            request.PropertyType,
            request.YearBuilt
        );

        var result = await _mediator.Send(command);

        var propertyId = await _queries.GetMostRecentPropertyIdByOwnerAsync(request.OwnerId);

        if (request.Images != null && request.Images.Any())
        {
            foreach (var image in request.Images)
            {
                var imageCommand = new AddPropertyImageCommand(
                    propertyId,
                    image.ImageData,
                    image.ImageType,
                    image.Caption ?? string.Empty
                );
                await _mediator.Send(imageCommand);
            }
        }

        await _searchEngine.InitializeAsync();

        return Ok(new { PropertyId = propertyId });
    }

    [HttpGet("{propertyId}")]
    public async Task<IActionResult> GetPropertyById(int propertyId)
    {
        _logger.LogInformation("Getting property with ID: {PropertyId}", propertyId);

        var property = await _queries.GetPropertyByIdAsync(propertyId);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProperties()
    {
        _logger.LogInformation("Getting all properties");

        var properties = await _queries.GetAllPropertiesAsync();

        if (properties == null || !properties.Any())
        {
            return NotFound();
        }

        return Ok(properties);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetPropertiesByStatus(string status)
    {
        _logger.LogInformation("Getting properties with status: {Status}", status);

        if (!PropertyStatus.AllStatuses.Contains(status))
        {
            return BadRequest("Invalid property status");
        }

        var properties = await _queries.GetPropertiesByStatusAsync(status);

        if (properties == null || !properties.Any())
        {
            return NotFound();
        }

        return Ok(properties);
    }

    [HttpPut("{propertyId}")]
    public async Task<IActionResult> UpdateProperty(int propertyId, [FromBody] UpdatePropertyRequest request)
    {
        _logger.LogInformation("Updating property with ID: {PropertyId}", propertyId);

        var command = new UpdatePropertyCommand(
            propertyId,
            request.Title,
            request.Description,
            request.Price,
            request.Bedrooms,
            request.Bathrooms,
            request.SquareFootage,
            request.HasBalcony,
            request.HasParking,
            request.PetsAllowed,
            request.PropertyType,
            request.YearBuilt
        );

        var result = await _mediator.Send(command);

        if (result)
        {
            await _searchEngine.InitializeAsync();
            return Ok();
        }

        return NotFound();
    }

    [HttpPut("{propertyId}/status/available")]
    public async Task<IActionResult> MarkPropertyAsAvailable(int propertyId)
    {
        _logger.LogInformation("Marking property as available - PropertyId: {PropertyId}", propertyId);

        var command = new MarkPropertyAsAvailableCommand(propertyId);
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return NotFound();
    }

    [HttpPut("{propertyId}/status/unavailable")]
    public async Task<IActionResult> MarkPropertyAsUnavailable(int propertyId)
    {
        _logger.LogInformation("Marking property as unavailable - PropertyId: {PropertyId}", propertyId);

        var command = new MarkPropertyAsUnavailableCommand(propertyId);
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return NotFound();
    }

    [HttpPost("{propertyId}/images")]
    public async Task<IActionResult> AddPropertyImage(int propertyId, [FromForm] AddPropertyImageRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
        {
            return BadRequest("No image was uploaded");
        }

        if (request.Image.Length > 5 * 1024 * 1024)
        {
            return BadRequest("Image size exceeds 5MB limit");
        }

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        if (!allowedContentTypes.Contains(request.Image.ContentType))
        {
            return BadRequest("Only JPG, PNG, and GIF images are allowed");
        }

        using var memoryStream = new MemoryStream();
        await request.Image.CopyToAsync(memoryStream);
        var imageData = memoryStream.ToArray();

        var command = new AddPropertyImageCommand(
            propertyId,
            imageData,
            request.Image.ContentType,
            request.Caption
        );

        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return BadRequest("Failed to upload image");
    }

    [HttpGet("{propertyId}/image")]
    public async Task<IActionResult> GetPropertyImage(int propertyId)
    {
        var image = await _queries.GetPropertyImageAsync(propertyId);

        if (image == null)
        {
            return NotFound();
        }

        return File(image.ImageData, image.ImageType);
    }

    [HttpDelete("{propertyId}/images/{imageId}")]
    public async Task<IActionResult> DeletePropertyImage(int propertyId, int imageId)
    {
        var command = new DeletePropertyImageCommand(propertyId, imageId);
        var result = await _mediator.Send(command);

        if (result)
        {
            return Ok();
        }

        return NotFound();
    }
    [HttpPost("similar")]
    public async Task<IActionResult> GetSimilarProperties([FromBody] SimilarPropertySearchRequest request)
    {
        _logger.LogInformation("Finding similar properties by criteria");

        try
        {
            var similarProperties = await _searchEngine.FindSimilarPropertiesAsync(
                price: request.Price,
                latitude: request.Latitude,
                longitude: request.Longitude,
                squareFootage: request.SquareFootage,
                bedrooms: request.Bedrooms,
                bathrooms: request.Bathrooms,
                yearBuilt: request.YearBuilt,
                hasBalcony: request.HasBalcony,
                hasParking: request.HasParking,
                petsAllowed: request.PetsAllowed,
                propertyType: request.PropertyType,
                maxResults: request.MaxResults ?? 12
            );
            if (similarProperties == null || !similarProperties.Any())
            {
                return Ok(Enumerable.Empty<Property>());
            }

            return Ok(similarProperties);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar properties");
            return BadRequest("An error occurred while finding similar properties.");
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetPropertyStatistics()
    {
        _logger.LogInformation("Getting property statistics");

        try
        {
            var properties = await _queries.GetAllPropertiesAsync();

            if (properties == null || !properties.Any())
            {
                return NotFound("No properties found for statistics calculation");
            }

            var stats = new
            {
                TotalCount = properties.Count(),
                AveragePrice = properties.Average(p => p.Price),
                MinPrice = properties.Min(p => p.Price),
                MaxPrice = properties.Max(p => p.Price),
                AverageSquareFootage = properties.Average(p => p.SquareFootage),
                AverageBedrooms = properties.Average(p => p.Bedrooms),
                AverageBathrooms = properties.Average(p => p.Bathrooms),
                NewestProperty = properties.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                OldestProperty = properties.OrderBy(p => p.CreatedAt).FirstOrDefault(),
                PropertyTypeCounts = properties.GroupBy(p => p.PropertyType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property statistics");
            return BadRequest("An error occurred while calculating property statistics.");
        }
    }

    [HttpGet("initialize-search-engine")]
    public async Task<IActionResult> InitializeSearchEngine()
    {
        _logger.LogInformation("Initializing property search engine");

        try
        {
            await _searchEngine.InitializeAsync();
            return Ok("Search engine initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing search engine");
            return BadRequest("Failed to initialize property search engine");
        }
    }

    [HttpPost("train")]
    public async Task<IActionResult> TrainModel()
    {
        _logger.LogInformation("Training KNN property model");

        try
        {
            await _searchEngine.TrainKNNModelAsync();
            return Ok(new { Message = "KNN model successfully trained" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training KNN model");
            return BadRequest(new { Error = "Failed to train the model", Details = ex.Message });
        }
    }

    [HttpGet("evaluate")]
    public async Task<IActionResult> EvaluateModel()
    {
        _logger.LogInformation("Evaluating property search model effectiveness");

        try
        {
            var results = await _searchEngine.EvaluateModelEffectivenessAsync();

            if (!results.Success)
            {
                return BadRequest(new { Error = results.ErrorMessage });
            }

            return Ok(new
            {
                Metrics = new
                {
                    Precision = results.Precision,
                    Recall = results.Recall,
                    F1Score = results.F1Score,
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating search model");
            return BadRequest(new { Error = "Failed to evaluate the model", Details = ex.Message });
        }
    }
}