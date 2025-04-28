namespace Bookings.Api.Controllers.Request.Properties;

public record CreatePropertyRequest(
    int OwnerId,
    string Title,
    string Description,
    double Latitude,
    double Longitude,
    decimal Price,
    int Bedrooms,
    int Bathrooms,
    float SquareFootage,
    bool HasBalcony,
    bool HasParking,
    bool PetsAllowed,
    string PropertyType,
    int YearBuilt,
    List<PropertyImageRequest>? Images
);
