using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed class LoggingBus(ILogger<LoggingBus> logger) : IMessageBus
    {
        public Task PublishAsync(IntegrationMessage message, CancellationToken ct)
        {
            logger.LogInformation("Publishing event {Name} payload={@Payload}",
                    message.Name, message.Payload);

            return Task.CompletedTask;
        }
    }
}
