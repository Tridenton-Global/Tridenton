namespace Tridenton.Core.Exceptions;

public class BadRequestException : TridentonException
{
    public BadRequestException(bool hideStackTrace = true, bool hideSource = true) : base(ErrorSide.Sender, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.BadRequest;
    }

    public BadRequestException(string? message, bool hideStackTrace = true, bool hideSource = true) : base(message, ErrorSide.Sender, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.BadRequest;
    }
}