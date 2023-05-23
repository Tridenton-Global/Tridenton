namespace Tridenton.CQRS;

public interface IPipelineContext<TRequest, TResponse> : IRequestContext<TRequest, TResponse>
    where TRequest : TridentonRequest<TResponse>
    where TResponse : class
{
    RequestHandlerDelegate<TResponse> Next { get; }
}

internal sealed record PipelineContext<TRequest, TResponse>(
    TRequest Request,
    RequestHandlerDelegate<TResponse> Next,
    CancellationToken CancellationToken) : IPipelineContext<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class;