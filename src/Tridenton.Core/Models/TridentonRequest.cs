namespace Tridenton.Core.Models;

public abstract class TridentonRequest
{
    public virtual bool Anonymous() => false;
}

public abstract class TridentonRequest<TResponse> : TridentonRequest where TResponse : class { }