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
        var handlersTypes = ServicesRegistrar.Instance.GetNotificationHandlers(notification.GetType());

        for (long i = 0; i < handlersTypes.LongLength; i++)
        {
            var handler = GetService<NotificationHandler<TNotification>>(handlersTypes[i]);

            handler.HandleAsync(new NotificationContext<TNotification>(notification, cancellationToken)).ConfigureAwait(false);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : TridentonRequest
    {
        var handlerType = ServicesRegistrar.Instance.GetRequestHandler(request.GetType());

        var handler = GetService<RequestHandler<TRequest>>(handlerType);

        return handler.HandleAsync(new RequestContext<TRequest>(request, cancellationToken));
    }

    public ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest<TResponse>
        where TResponse : class
    {
        var handlerType = ServicesRegistrar.Instance.GetRequestHandler(request.GetType());

        var handler = GetService<RequestHandler<TRequest, TResponse>>(handlerType);

        return handler.HandleAsync(new RequestContext<TRequest, TResponse>(request, cancellationToken));
    }
}