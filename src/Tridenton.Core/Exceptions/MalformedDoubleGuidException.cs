namespace Tridenton.Core.Exceptions;

public class MalformedDoubleGuidException : BadRequestException
{
    private const string Header = "Value has wrong format. Double guid is represented in ########-####-####-####-############-########-####-####-####-############ format";

    public MalformedDoubleGuidException() : base(Header) { }
}