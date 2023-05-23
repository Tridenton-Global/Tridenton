namespace Tridenton.Core.Exceptions;

public sealed class FailedResponseException : TridentonException
{
    public FailedResponseException(string message, HttpStatusCode statusCode, ErrorSide errorSide, bool hideStackTrace = true, bool hideSource = true)
        : base(message, errorSide, hideStackTrace, hideSource)
    {
        StatusCode = statusCode;
    }
}