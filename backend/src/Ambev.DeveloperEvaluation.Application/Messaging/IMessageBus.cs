namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public interface IMessageBus
    {
        Task PublishMessagesAsync(IEnumerable<IntegrationMessage> messages, CancellationToken ct);
        Task PublishAsync(IntegrationMessage message, CancellationToken ct);
    }
}
