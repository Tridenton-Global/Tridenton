namespace Tridenton.Persistence;

public class InvalidExpressionOperatorException : BadRequestException
{
    public InvalidExpressionOperatorException(FilteringExpression expression, bool hideStackTrace = true, bool hideSource = true)
        : base($"Operator '{expression.Operator}' is not valid for property {expression.Property}", hideStackTrace, hideSource) { }
}