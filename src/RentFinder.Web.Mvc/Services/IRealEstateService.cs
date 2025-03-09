using RentFinder.Web.Mvc.Models;

namespace RentFinder.Web.Mvc.Services;
public interface IRealEstateService
{
    Task<IEnumerable<PropertyViewModel>> SearchPropertiesAsync(PropertySearchModel searchModel);
    Task<PropertyDetailViewModel> GetPropertyDetailAsync(int propertyId);
    Task<IEnumerable<PropertyViewModel>> GetRecommendedPropertiesAsync();
}
