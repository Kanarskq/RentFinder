using MediatR;

namespace Bookings.Api.Application.Commands.Properties;
public record UpdatePropertyCommand(
    int PropertyId,
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
    int YearBuilt) : IRequest<bool>;
