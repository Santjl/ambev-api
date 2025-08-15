using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed class LoggingBus(ILogger<LoggingBus> logger) : IMessageBus
    {
        public Task PublishAsync(IntegrationMessage[] messages, CancellationToken ct)
        {
            foreach (var message in messages)
            {
                logger.LogInformation("Publishing event {Name} payload={@Payload}",
                    message.Name, message.Payload);
            }
            

            return Task.CompletedTask;
        }
    }
}
