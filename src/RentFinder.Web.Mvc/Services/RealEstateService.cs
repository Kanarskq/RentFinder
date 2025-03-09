using RentFinder.Web.Mvc.Models;

namespace RentFinder.Web.Mvc.Services;

public class RealEstateService : IRealEstateService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RealEstateService> _logger;

    public RealEstateService(HttpClient httpClient, ILogger<RealEstateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<PropertyViewModel>> SearchPropertiesAsync(PropertySearchModel searchModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/properties/search", searchModel);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching properties");
            return Enumerable.Empty<PropertyViewModel>();
        }
    }

    public async Task<PropertyDetailViewModel> GetPropertyDetailAsync(int propertyId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/properties/{propertyId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PropertyDetailViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting property details for {PropertyId}", propertyId);
            return null;
        }
    }

    public async Task<IEnumerable<PropertyViewModel>> GetRecommendedPropertiesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/properties/recommended");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommended properties");
            return Enumerable.Empty<PropertyViewModel>();
        }
    }
}
