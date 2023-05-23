namespace Tridenton.Core.Exceptions;

public class NotFoundException : TridentonException
{
    public NotFoundException(bool hideStackTrace = true, bool hideSource = true)
        : base(ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.NotFound;
    }

    public NotFoundException(string? message, bool hideStackTrace = true, bool hideSource = true)
        : base(message, ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}