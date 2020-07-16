using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        void AddSubscription<T, TH>()
          where T : IntegrationEvent
          where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
    }
}
