using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<CreatePropertyCommandHandler> _logger;

    public CreatePropertyCommandHandler(IPropertyRepository propertyRepository, ILogger<CreatePropertyCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = new Property(
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

            _propertyRepository.Add(property);

            return await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property for owner {OwnerId}", request.OwnerId);
            return false;
        }
    }
}
