namespace Tridenton.Persistence;

public class InvalidFilterArgumentException : BadRequestException
{
    public InvalidFilterArgumentException(string value, FilteringExpression expression, bool hideStackTrace = true, bool hideSource = true)
        : base($"Value '{value}' is not valid for {expression.Property}", hideStackTrace, hideSource) { }
}