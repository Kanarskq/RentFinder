using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public class MarkPropertyAsUnavailableCommandHandler : IRequestHandler<MarkPropertyAsUnavailableCommand, bool>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<MarkPropertyAsUnavailableCommandHandler> _logger;

    public MarkPropertyAsUnavailableCommandHandler(IPropertyRepository propertyRepository, ILogger<MarkPropertyAsUnavailableCommandHandler> logger)
    {
        _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(MarkPropertyAsUnavailableCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetAsync(request.PropertyId);
            if (property == null)
            {
                _logger.LogWarning("Property {PropertyId} not found", request.PropertyId);
                return false;
            }

            property.MarkAsUnavailable();

            _propertyRepository.Update(property);

            return await _propertyRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking property {PropertyId} as unavailable", request.PropertyId);
            return false;
        }
    }
}
