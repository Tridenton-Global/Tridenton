using System.Runtime.CompilerServices;

namespace Tridenton.Core.Util;

public abstract class Awaiter
{
    public abstract TaskAwaiter GetAwaiter();
}