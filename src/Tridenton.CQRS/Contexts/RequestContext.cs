namespace Tridenton.CQRS;

public interface IRequestContext<TRequest> : IContextBase where TRequest : TridentonRequest
{
    TRequest Request { get; }
}

public interface IRequestContext<TRequest, TResponse> : IRequestContext<TRequest> where TRequest : TridentonRequest<TResponse> where TResponse : class { }

internal sealed record RequestContext<TRequest>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest> where TRequest : TridentonRequest;

internal sealed record RequestContext<TRequest, TResponse>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class;