using System.Collections.Concurrent;

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

internal sealed class Orchestrator : IOrchestrator
{
    private readonly ConcurrentDictionary<Type, Type> _requestsHandlers;

    public Orchestrator()
    {
        _requestsHandlers = new();
    }

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : TridentonRequest
    {
        throw new NotImplementedException();
    }

    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : TridentonRequest
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest<TResponse>
        where TResponse : class
    {
        throw new NotImplementedException();
    }
}