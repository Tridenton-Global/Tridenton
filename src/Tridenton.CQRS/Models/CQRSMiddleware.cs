namespace Tridenton.CQRS;

public abstract class CQRSMiddleware<TRequest, TResponse> : AbstractService where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    protected CQRSMiddleware(IServiceProvider services) : base(services) { }

    protected abstract ValueTask<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context);
}