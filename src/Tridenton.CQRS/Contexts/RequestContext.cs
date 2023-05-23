namespace Tridenton.CQRS;

public interface IRequestContextBase : ICancelable
{
    DoubleGuid RequestID => DoubleGuid.NewGuid();

    DateTime RequestTS => DateTime.UtcNow;
}

public interface IRequestContext<TRequest> : IRequestContextBase
    where TRequest : TridentonRequest
{
    TRequest Request { get; }
}

internal sealed record RequestContext<TRequest>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest>
    where TRequest : TridentonRequest;

public interface IRequestContext<TRequest, TResponse> : IRequestContextBase
    where TRequest : TridentonRequest<TResponse>
    where TResponse : class
{
    TRequest Request { get; }
}

internal sealed record RequestContext<TRequest, TResponse>(TRequest Request, CancellationToken CancellationToken) : IRequestContext<TRequest, TResponse>
    where TRequest : TridentonRequest<TResponse>
    where TResponse : class;