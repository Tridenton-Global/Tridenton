namespace Tridenton.CQRS;

public interface IMiddlewareContext<TRequest, TResponse> : IRequestContext<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    RequestHandlerDelegate<TResponse> Next { get; }
}

internal sealed record MiddlewareContext<TRequest, TResponse>(TRequest Request, RequestHandlerDelegate<TResponse> Next, CancellationToken CancellationToken)
    : IMiddlewareContext<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class;