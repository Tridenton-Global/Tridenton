namespace Tridenton.CQRS;

public abstract class CQRSMiddleware<TRequest, TResponse> : AbstractService where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    protected CQRSMiddleware(IServiceProvider services) : base(services) { }

    public abstract Task<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context);
}

public abstract class CQRSMiddleware<TRequest> : AbstractService where TRequest : TridentonRequest
{
    protected CQRSMiddleware(IServiceProvider services) : base(services) { }

    public abstract Task HandleAsync(IMiddlewareContext<TRequest> context);
}