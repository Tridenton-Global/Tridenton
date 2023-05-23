namespace Tridenton.Extensions;

public static class CollectionsExtensions
{
    /// <summary>
    ///     Counts the total amount of elements in <paramref name="enumerable"/>
    /// </summary>
    /// <param name="enumerable">Input source</param>
    /// <returns>
    ///     Total amount of elements in <paramref name="enumerable"/>
    /// </returns>
    public static long Count(this IEnumerable enumerable)
    {
        long count = 0;

        foreach (var item in enumerable)
        {
            count++;
        }

        return count;
    }
}