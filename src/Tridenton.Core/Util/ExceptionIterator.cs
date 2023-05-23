namespace Tridenton.Core.Util;

public readonly struct ExceptionIterator
{
    public static Exception HandleExceptionRecursively(Exception exception)
    {
        if (exception is not null)
        {
            while (exception.InnerException is not null)
            {
                exception = exception.InnerException;
            }
        }

        return exception!;
    }
}