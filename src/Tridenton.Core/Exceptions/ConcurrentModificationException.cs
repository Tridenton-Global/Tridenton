namespace Tridenton.Core.Exceptions;

public class ConcurrentModificationException : TridentonException
{
    public ConcurrentModificationException(bool hideStackTrace = true, bool hideSource = true)
        : base(ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Conflict;
    }

    public ConcurrentModificationException(string? message, bool hideStackTrace = true, bool hideSource = true)
        : base(message, ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
}