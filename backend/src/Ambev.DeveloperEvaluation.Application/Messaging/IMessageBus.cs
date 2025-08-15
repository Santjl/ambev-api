namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync(IntegrationMessage[] messages, CancellationToken ct);
    }
}
