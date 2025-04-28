namespace Bookings.Api.Controllers.Request.Properties;

public record UpdatePropertyRequest(
    string Title,
    string Description,
    decimal Price,
    int Bedrooms,
    int Bathrooms,
    float SquareFootage,
    bool HasBalcony,
    bool HasParking,
    bool PetsAllowed,
    string PropertyType,
    int YearBuilt
);