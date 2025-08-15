namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync(IntegrationMessage message, CancellationToken ct);
    }
}
