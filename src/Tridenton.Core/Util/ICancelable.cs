namespace Tridenton.Core.Util;

public interface ICancelable
{
    CancellationToken CancellationToken { get; }
}