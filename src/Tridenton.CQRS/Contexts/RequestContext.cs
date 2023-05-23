namespace Tridenton.CQRS;

public interface IRequestContext<TRequest> where TRequest : TridentonRequest
{
    DoubleGuid RequestID => DoubleGuid.NewGuid();

    DateTime RequestTS => DateTime.UtcNow;

    TRequest Request { get; }
}

public interface IRequestContext<TRequest, TResponse> : IRequestContext<TRequest> where TRequest : TridentonRequest<TResponse> where TResponse : class { }

internal sealed record RequestContext<TRequest>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest> where TRequest : TridentonRequest;

internal sealed record RequestContext<TRequest, TResponse>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class;