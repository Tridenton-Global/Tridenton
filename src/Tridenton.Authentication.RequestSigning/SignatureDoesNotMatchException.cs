namespace Tridenton.Authentication.RequestSigning;

public class SignatureDoesNotMatchException : BadRequestException
{
    private const string Header = "The request signature we calculated does not match the signature you provided. Check your Secret Access Key and signing method.";

    public SignatureDoesNotMatchException(bool hideStackTrace = true, bool hideSource = true) : base(Header, hideStackTrace, hideSource) { }
}