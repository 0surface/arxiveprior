using EventBus.Events;
using System.Threading.Tasks;

namespace EventBus.Abstractions
{
    public interface IIntgeationEventHandler<in TInterationEvent>  : IIntegrationEventHandler
        where TInterationEvent : IntegrationEvent
    {
        Task Handle(TInterationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}