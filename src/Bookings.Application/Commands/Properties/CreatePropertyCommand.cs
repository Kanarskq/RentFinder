using MediatR;

namespace Bookings.Application.Commands.Properties;

public record CreatePropertyCommand(
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
    int YearBuilt) : IRequest<bool>;


