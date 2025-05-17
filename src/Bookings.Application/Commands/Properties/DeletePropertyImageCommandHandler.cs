using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Commands.Properties;

public class DeletePropertyImageCommandHandler : IRequestHandler<DeletePropertyImageCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<DeletePropertyImageCommandHandler> _logger;

    public DeletePropertyImageCommandHandler(IPropertyRepository propertyRepository, ILogger<DeletePropertyImageCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeletePropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetAsync(request.PropertyId);
            if (property == null)
            {
                _logger.LogWarning("Property {PropertyId} not found", request.PropertyId);
                return false;
            }

            property.RemoveImage(request.ImageId);

            _propertyRepository.Update(property);

            return await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing image {ImageId} from property {PropertyId}",
                request.ImageId, request.PropertyId);
            return false;
        }
    }
}
