namespace Tridenton.CQRS;

public sealed class MoreThanOneRequestHandlerException : Exception
{
	public MoreThanOneRequestHandlerException(Type requestType) : base($"Unable to set up CQRS request handlers. Request '{requestType.Name}' has more than one handler") { }
}