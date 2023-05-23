namespace Tridenton.Core.Exceptions;

public class InvalidRequestException : BadRequestException
{
    public InvalidRequestException(string error, bool hideStackTrace = true, bool hideSource = true)
        : this(new string[] { error }, hideStackTrace, hideSource) { }

    public InvalidRequestException(string[] validationErrors, bool hideStackTrace = true, bool hideSource = true)
        : base(BuildErrorMessage(validationErrors), hideStackTrace, hideSource) { }

    private static string BuildErrorMessage(string[] validationErrors)
    {
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine("Request contains one or more validation errors:");

        strBuilder.Append(string.Join('\n', validationErrors.Select(e => $"- {e}")));

        return strBuilder.ToString();
    }
}