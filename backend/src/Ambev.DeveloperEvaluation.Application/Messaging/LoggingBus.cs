using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed class LoggingBus(ILogger<LoggingBus> logger) : IMessageBus
    {
        public Task PublishMessagesAsync(IEnumerable<IntegrationMessage> messages, CancellationToken ct)
        {
            foreach (var message in messages)
            {
                PublishAsync(message, ct);
            }
            

            return Task.CompletedTask;
        }

        public Task PublishAsync(IntegrationMessage message, CancellationToken ct)
        {
            logger.LogInformation("Publishing event {Name} payload={@Payload}",
                    message.Name, message.Payload);
            return Task.CompletedTask;
        }
    }
}
