namespace Tridenton.CQRS;

public interface IOrchestrator
{
    ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest;

    ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest<TResponse>
        where TResponse : class;

    ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : TridentonRequest;
}