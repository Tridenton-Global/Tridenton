using System.Collections.Concurrent;
using System.Linq;

namespace Tridenton.CQRS;

public interface IOrchestrator
{
    ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest;

    ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest<TResponse>
        where TResponse : class;

    ValueTask PublishAsync<TNotification>(TNotification notification, PublicationBehavior? behavior = null, CancellationToken cancellationToken = default)
        where TNotification : TridentonRequest;
}

internal sealed class Orchestrator : AbstractService, IOrchestrator
{
    public Orchestrator(IServiceProvider services) : base(services) { }

    public async ValueTask PublishAsync<TNotification>(TNotification notification, PublicationBehavior? behavior = null, CancellationToken cancellationToken = default) where TNotification : TridentonRequest
    {
        behavior ??= ServicesRegistrar.Instance.PublicationBehavior;

        var handlersTypes = ServicesRegistrar.Instance.GetNotificationHandlers(notification.GetType());

        var tasks = new List<Task>();

        for (long i = 0; i < handlersTypes.LongLength; i++)
        {
            var handler = GetService<NotificationHandler<TNotification>>(handlersTypes[i]);

            var task = handler.HandleAsync(new NotificationContext<TNotification>(notification, cancellationToken));

            switch (behavior)
            {
                case PublicationBehavior.Parallel:
                    tasks.Add(task);
                    break;

                case PublicationBehavior.Sequential:
                    await task.ConfigureAwait(false);
                    break;

                default:
                    break;
            }
        }

        if (tasks.Any())
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    public async ValueTask SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : TridentonRequest
    {
        var handlerType = ServicesRegistrar.Instance.GetRequestHandler(request.GetType());

        var handler = GetService<RequestHandler<TRequest>>(handlerType);

        RequestHandlerDelegate handlerDelegate = () => handler.HandleAsync(new RequestContext<TRequest>(request, cancellationToken)).AsTask();

        var response = GetServices<CQRSMiddleware<TRequest>>()
            .Reverse()
            .Aggregate(handlerDelegate, (next, middleware) => () => middleware.HandleAsync(new MiddlewareContext<TRequest>(request, next, cancellationToken)))
            .Invoke();

        await response;
    }

    public async ValueTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : TridentonRequest<TResponse>
        where TResponse : class
    {
        var handlerType = ServicesRegistrar.Instance.GetRequestHandler(request.GetType());

        var handler = GetService<RequestHandler<TRequest, TResponse>>(handlerType);

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler!.HandleAsync(new RequestContext<TRequest, TResponse>(request, cancellationToken)).AsTask();

        var response = GetServices<CQRSMiddleware<TRequest, TResponse>>()
            .Reverse()
            .Aggregate(handlerDelegate, (next, middleware) => () => middleware.HandleAsync(new MiddlewareContext<TRequest, TResponse>(request, next, cancellationToken)))
            .Invoke();

        return await response;
    }
}