namespace Tridenton.CQRS;

public abstract class RequestHandlerBase : AbstractService
{
    protected readonly IOrchestrator Orchestrator;

    protected RequestHandlerBase(IServiceProvider services) : base(services)
    {
        Orchestrator = GetService<IOrchestrator>();
    }
}

public abstract class RequestHandler<TRequest> : RequestHandlerBase where TRequest : TridentonRequest
{
    protected RequestHandler(IServiceProvider services) : base(services) { }

    protected abstract ValueTask HandleAsync(IRequestContext<TRequest> context);
}

public abstract class RequestHandler<TRequest, TResponse> : RequestHandlerBase
    where TRequest : TridentonRequest<TResponse>
    where TResponse : class
{
    protected RequestHandler(IServiceProvider services) : base(services) { }

    protected abstract ValueTask<TResponse> HandleAsync(IRequestContext<TRequest, TResponse> context);
}