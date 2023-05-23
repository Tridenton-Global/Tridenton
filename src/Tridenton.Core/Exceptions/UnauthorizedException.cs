namespace Tridenton.Core.Exceptions;

public class UnauthorizedException : TridentonException
{
    public UnauthorizedException(bool hideStackTrace = true, bool hideSource = true) : base(ErrorSide.Receiver, hideStackTrace, hideSource)
    {
        StatusCode = HttpStatusCode.Unauthorized;
    }
}