namespace Tridenton.Core.Exceptions;

public class TridentonException : Exception
{
    private readonly bool _hideStackTrace;
    private readonly bool _hideSource;

    public HttpStatusCode StatusCode { get; protected set; }

    public ErrorSide ErrorSide { get; protected set; }

    public override string? StackTrace => _hideStackTrace ? null : base.StackTrace;

    public override string? Source => _hideSource ? null : base.Source;

    public TridentonException(ErrorSide errorSide, bool hideStackTrace = true, bool hideSource = true)
    {
        ErrorSide = errorSide;

        _hideStackTrace = hideStackTrace;
        _hideSource = hideSource;
    }

    public TridentonException(string? message, ErrorSide errorSide, bool hideStackTrace = true, bool hideSource = true) : base(message)
    {
        ErrorSide = errorSide;

        _hideStackTrace = hideStackTrace;
        _hideSource = hideSource;
    }
}