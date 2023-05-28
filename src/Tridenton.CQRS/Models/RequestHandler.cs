namespace Tridenton.CQRS;

public abstract class RequestHandler<TRequest> : BaseHandler where TRequest : TridentonRequest
{
    protected RequestHandler(IServiceProvider services) : base(services) { }

    public abstract ValueTask HandleAsync(IRequestContext<TRequest> context);
}

public abstract class RequestHandler<TRequest, TResponse> : BaseHandler
    where TRequest : TridentonRequest<TResponse>
    where TResponse : class
{
    protected RequestHandler(IServiceProvider services) : base(services) { }

    public abstract ValueTask<TResponse> HandleAsync(IRequestContext<TRequest, TResponse> context);
}