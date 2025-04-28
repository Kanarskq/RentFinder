using Bookings.Domain.AggregatesModel.PropertyAggregate;

namespace Bookings.Api.Infrastructure.Services.Search;

public interface IPropertySearchEngine
{
    Task InitializeAsync();
    Task<IEnumerable<Property>> FindSimilarPropertiesAsync(decimal price,
        double latitude,
        double longitude,
        float squareFootage,
        int bedrooms,
        float bathrooms,
        int yearBuilt,
        bool hasBalcony,
        bool hasParking,
        bool petsAllowed,
        string propertyType,
        int maxResults = 12);
    Task TrainKNNModelAsync();
    Task<ModelEvaluationResults> EvaluateModelEffectivenessAsync();
}
