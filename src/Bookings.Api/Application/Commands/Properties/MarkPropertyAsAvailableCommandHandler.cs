using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public class MarkPropertyAsAvailableCommandHandler : IRequestHandler<MarkPropertyAsAvailableCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<MarkPropertyAsAvailableCommandHandler> _logger;

    public MarkPropertyAsAvailableCommandHandler(IPropertyRepository propertyRepository, ILogger<MarkPropertyAsAvailableCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(MarkPropertyAsAvailableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetAsync(request.PropertyId);
            if (property == null)
            {
                _logger.LogWarning("Property {PropertyId} not found", request.PropertyId);
                return false;
            }

            property.MarkAsAvailable();

            _propertyRepository.Update(property);

            return await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking property {PropertyId} as available", request.PropertyId);
            return false;
        }
    }
}
