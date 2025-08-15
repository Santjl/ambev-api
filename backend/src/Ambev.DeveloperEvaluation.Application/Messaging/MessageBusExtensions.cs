using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public static class MessageBusExtensions
    {
        public static Task PublishEventsAsync(
            this IMessageBus bus,
            IEnumerable<object> events,
            CancellationToken ct)
        {
            var msgs = events.Select(Build).ToArray();
            return bus.PublishAsync(msgs, ct);

            IntegrationMessage Build(object ev)
            {
                var type = ev.GetType();
                var route = type.GetCustomAttribute<EventRouteAttribute>()?.Name
                            ?? type.FullName!;

                var payload = ev is IEventPayload p ? p.ToPayload() : ev;

                return new IntegrationMessage(
                    route,
                    payload,
                    DateTimeOffset.UtcNow
                );
            }
        }

        public static Task PublishEventAsync(
            this IMessageBus bus,
            object ev,
            CancellationToken ct) =>
            bus.PublishEventsAsync(new[] { ev }, ct);
    }
}
