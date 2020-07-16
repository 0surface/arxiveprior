using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;

namespace EventBusServiceBus
{
    public class AzureServiceBus : IEventBus
    {
        private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
        private readonly ILogger<AzureServiceBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "arx_event_bus";
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public AzureServiceBus(IServiceBusPersisterConnection serviceBusPersisterConnection,
            ILogger<AzureServiceBus> logger, IEventBusSubscriptionsManager subsManager, string subscriptionClientName,
            ILifetimeScope autofac)
        {
            _serviceBusPersisterConnection = serviceBusPersisterConnection;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();

            _subscriptionClient = new SubscriptionClient(serviceBusPersisterConnection.ServiceBusConnectionStringBuilder,
                subscriptionClientName);
            _autofac = autofac;
        }
        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                try
                {
                    _subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter { Label = eventName },
                        Name = eventName
                    }).GetAwaiter().GetResult();
                }
                catch (ServiceBusException)
                {
                    _logger.LogWarning("The messaging entity {eventName} already exists.", eventName);
                }
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subsManager.AddSubscription<T, TH>();
        }
    }
}
