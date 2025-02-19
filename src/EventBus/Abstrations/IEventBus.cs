using EventBus.Events;

namespace EventBus.Abstrations;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
