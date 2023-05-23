namespace Tridenton.Core.Exceptions;

public class AccessDeniedException : TridentonException
{
    public AccessDeniedException(bool hideStackTrace = true, bool hideSource = true) : base(ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Forbidden;
    }

    public AccessDeniedException(string? message, bool hideStackTrace = true, bool hideSource = true) : base(message, ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Forbidden;
    }
}