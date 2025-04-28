using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<UpdatePropertyCommandHandler> _logger;

    public UpdatePropertyCommandHandler(IPropertyRepository propertyRepository, ILogger<UpdatePropertyCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetAsync(request.PropertyId);
            if (property == null)
            {
                _logger.LogWarning("Property {PropertyId} not found", request.PropertyId);
                return false;
            }

            property.UpdateDetails(
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

            _propertyRepository.Update(property);

            return await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property {PropertyId}", request.PropertyId);
            return false;
        }
    }
}
