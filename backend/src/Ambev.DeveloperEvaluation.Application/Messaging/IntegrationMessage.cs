namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed record IntegrationMessage(
    string Name,
    object Payload,
    DateTimeOffset OccurredAt);
}
