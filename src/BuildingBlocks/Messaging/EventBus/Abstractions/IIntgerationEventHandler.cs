using EventBus.Events;
using System.Threading.Tasks;

namespace EventBus.Abstractions
{
    public interface IIntgerationEventHandler<in TInterationEvent>  : IIntegrationEventHandler
        where TInterationEvent : IntegrationEvent
    {
        Task Handle(TInterationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}