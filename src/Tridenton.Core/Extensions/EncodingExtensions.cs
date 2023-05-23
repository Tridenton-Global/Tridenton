using System.Globalization;

namespace Tridenton.Extensions;

public static class EncodingExtensions
{
    private const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

    /// <summary>
    ///     Encodes string to base64 representation
    /// </summary>
    /// <param name="value">Original string</param>
    /// <returns>
    ///     Base64 format string
    /// </returns>
    public static ReadOnlySpan<char> ToBase64(this string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value).AsSpan();
        return Convert.ToBase64String(bytes).AsSpan();
    }

    /// <summary>
    ///     Encodes byte array to base64 representation
    /// </summary>
    /// <param name="bytes">Byte array</param>
    /// <returns>
    ///     Base64 format string
    /// </returns>
    public static ReadOnlySpan<char> ToBase64(this byte[] bytes)
    {
        return bytes.ToBase64();
    }

    /// <summary>
    ///     Encodes byte array to base64 representation
    /// </summary>
    /// <param name="bytes">Byte array</param>
    /// <returns>
    ///     Base64 format string
    /// </returns>
    public static ReadOnlySpan<char> ToBase64(this ReadOnlySpan<byte> bytes)
    {
        return Convert.ToBase64String(bytes).AsSpan();
    }

    /// <summary>
    ///     Decodes base64 representation of string to its original form
    /// </summary>
    /// <param name="value">Base64 string</param>
    /// <returns>
    ///     Original string
    /// </returns>
    public static ReadOnlySpan<char> FromBase64(this string value)
    {
        var bytes = Convert.FromBase64String(value).AsSpan();
        return Encoding.UTF8.GetString(bytes).AsSpan();
    }

    /// <summary>
    ///     Decodes base64 representation of string to its original byte array
    /// </summary>
    /// <param name="value">Base64 string</param>
    /// <returns>
    ///     Original byte array
    /// </returns>
    public static ReadOnlySpan<byte> BytesFromBase64(this string value)
    {
        return Convert.FromBase64String(value).AsSpan();
    }

    /// <summary>
    ///     Converts string to base-16 format
    /// </summary>
    /// <param name="value">Input string</param>
    /// <returns>
    ///     Base16 format string
    /// </returns>
    public static ReadOnlySpan<char> ToHexString(this ReadOnlySpan<char> value)
    {
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < value.Length; i++)
        {
            var letter = value[i];

            stringBuilder.Append($"{Convert.ToInt32(letter):X}");
        }

        return stringBuilder.ToString().AsSpan();
    }

    /// <summary>
    ///     Converts string to base-16 format
    /// </summary>
    /// <param name="value">Input string</param>
    /// <returns>
    ///     Base16 format string
    /// </returns>
    public static ReadOnlySpan<char> ToHexString(this string value) => ToHexString(value.ToCharArray().AsSpan());

    /// <summary>
    ///     Encodes string value to URL format
    /// </summary>
    /// <param name="value">String value</param>
    /// <returns>
    ///     URL format encoded string
    /// </returns>
    public static ReadOnlySpan<char> UrlEncode(this string value)
    {
        var encoded = new StringBuilder();

        var bytes = Encoding.UTF8.GetBytes(value).AsSpan();

        for (int i = 0; i < bytes.Length; i++)
        {
            var symbol = (char)bytes[i];

            if (ValidUrlCharacters.Contains(symbol))
            {
                encoded.Append(symbol);
            }
            else
            {
                encoded.Append('%').Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
            }
        }

        return encoded.ToString().AsSpan();
    }

    /// <summary>
    ///     Reads stream content
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>
    ///     Stream content
    /// </returns>
    public static async ValueTask<string> GetStringAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);

        try
        {
            return await reader.ReadToEndAsync(cancellationToken);
        }
        finally
        {
            reader.Dispose();
        }
    }
}