using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Abstrations;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
