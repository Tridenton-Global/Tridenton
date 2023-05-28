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

internal sealed class Orchestrator : AbstractService, IOrchestrator
{
    public Orchestrator(IServiceProvider services) : base(services) { }

    public ValueTask PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : TridentonRequest
    {
        var handlers = ServicesRegistrar.Instance.GetNotificationHandlers(notification.GetType());

        for (long i = 0; i < handlers.LongLength; i++)
        {

        }

        return ValueTask.CompletedTask;
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