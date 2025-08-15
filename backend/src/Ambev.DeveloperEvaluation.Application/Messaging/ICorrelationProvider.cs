namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public interface ICorrelationProvider
    {
        Guid CorrelationId { get; }
        Guid? CausationId { get; }
    }
}
