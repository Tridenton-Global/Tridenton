namespace Tridenton.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Defines whether specified string is either null, empty or whitespace
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns><see langword="true"/> if specified string is either null, empty or whitespace; otherwise - <see langword="false"/></returns>
    public static bool IsEmpty(this string? value) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Defines whether <paramref name="value"/> to lowercase contains <paramref name="text"/> to lowercase
    /// </summary>
    /// <param name="value">String value which may contain <paramref name="text"/></param>
    /// <param name="text">String value to find</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> to lowercase contains <paramref name="text"/> to lowercase; otherwise - <see langword="false"/></returns>
    public static bool ContainsIgnoreCase(this string value, string text) => value.ToLower().Contains(text.ToLower());

    /// <summary>
    /// Defines whether <paramref name="value"/> matches to regular expression, defined by <paramref name="pattern"/>
    /// </summary>
    /// <param name="value">String value</param>
    /// <param name="pattern">Regular expression pattern</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> matches to regular expression, defined by <paramref name="pattern"/>; otherwise - <see langword="false"/></returns>
    public static bool MatchesRegex(this string value, string pattern) => new Regex(pattern).IsMatch(value);

    /// <summary>
    /// Builds new <see cref="Guid"/> structure from <paramref name="value"/> string without dashes ("-")
    /// </summary>
    /// <param name="value">String representation of <see cref="Guid"/> without dashes ("-")</param>
    /// <returns>New instance of <see cref="Guid"/></returns>
    /// <exception cref="ArgumentException" />
    /// <exception cref="FormatException" />
    public static Guid ToGuid(this string value)
    {
        var first = string.Join(string.Empty, value.Take(8));
        var second = string.Join(string.Empty, value.Skip(8).Take(4));
        var third = string.Join(string.Empty, value.Skip(12).Take(4));
        var fourth = string.Join(string.Empty, value.TakeLast(4));

        value = string.Join("-", first, second, third, fourth).ToString();

        return Guid.Parse(value);
    }

    /// <summary>
    /// Generate random string with length of <paramref name="digits"/>
    /// </summary>
    /// <param name="digits">Number of characters in resulting string (optional, 40 by default)</param>
    /// <returns>Random string</returns>
    public static string GenerateSecretAccessKey(int digits = 40)
    {
        var random = Random.Shared;

        var buffer = new byte[digits / 2];
        random.NextBytes(buffer);

        var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());

        if (digits % 2 == 0) return result;

        return result + random.Next(16).ToString("X");
    }

    /// <summary>
    /// Retrieves indices of <paramref name="text"/> inside <paramref name="value"/>
    /// </summary>
    /// <param name="value">Text, where <paramref name="text"/> may be found at</param>
    /// <param name="text">Text to find</param>
    /// <returns>List of indices, defined by <see cref="IEnumerable{Int32}"/></returns>
    public static List<int> GetWordsIndices(this string value, string text)
    {
        var result = new List<int>();

        for (int i = value.IndexOf(text, StringComparison.OrdinalIgnoreCase); i > -1; i = value.IndexOf(text, i + 1, StringComparison.OrdinalIgnoreCase))
        {
            result.Add(i);
        }

        return result;
    }
}