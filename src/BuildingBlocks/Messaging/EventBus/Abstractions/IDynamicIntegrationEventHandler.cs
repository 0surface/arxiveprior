using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
