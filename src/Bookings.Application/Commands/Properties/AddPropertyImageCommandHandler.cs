using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Commands.Properties;

public class AddPropertyImageCommandHandler : IRequestHandler<AddPropertyImageCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<AddPropertyImageCommandHandler> _logger;

    public AddPropertyImageCommandHandler(
        IPropertyRepository propertyRepository,
        ILogger<AddPropertyImageCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(AddPropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetAsync(request.PropertyId);

            if (property == null)
            {
                _logger.LogWarning("Property {PropertyId} not found when trying to add image", request.PropertyId);
                return false;
            }

            property.AddImage(
                request.ImageData,
                request.ImageType,
                request.Caption
            );

            _propertyRepository.Update(property);

            var result = await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            _logger.LogInformation(
                "Image added to property {PropertyId}, Image Type: {ImageType}, Caption: {Caption}",
                request.PropertyId,
                request.ImageType,
                request.Caption);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding image to property {PropertyId}", request.PropertyId);
            return false;
        }
    }
}
