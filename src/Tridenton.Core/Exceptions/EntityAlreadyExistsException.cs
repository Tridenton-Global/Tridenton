namespace Tridenton.Core.Exceptions;

public class EntityAlreadyExistsException : TridentonException
{
    public EntityAlreadyExistsException(bool hideStackTrace = true, bool hideSource = true)
        : base(ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Conflict;
    }

    public EntityAlreadyExistsException(string? message, bool hideStackTrace = true, bool hideSource = true)
        : base(message, ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
}