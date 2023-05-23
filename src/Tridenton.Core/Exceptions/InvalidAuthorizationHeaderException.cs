namespace Tridenton.Core.Exceptions;

public class InvalidAuthorizationHeaderException : TridentonException
{
    private const string Header = "Authorization header is invalid";

    public InvalidAuthorizationHeaderException(bool hideStackTrace = true, bool hideSource = true)
        : base(Header, ErrorSide.Sender, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Unauthorized;
    }
}