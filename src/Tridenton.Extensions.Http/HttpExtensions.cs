using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Http;
using Tridenton.Core.Models;

namespace Tridenton.Extensions.Http;

public static class HttpExtensions
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="request"></param>
    /// <param name="header"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsRequestHeaderPresent(this HttpRequest request, string header, out string value)
    {
        var isPresent = request.Headers.TryGetValue(header, out var headerValue);

        value = headerValue.ToString();

        return isPresent;
    }

    /// <summary>
    ///     Converts <see cref="IQueryCollection"/> query parameters to canonical string
    /// </summary>
    /// <param name="queryParameters">Query parameters</param>
    /// <returns>Canonical string</returns>
    public static ReadOnlySpan<char> GetCanonicalQueryParameters(this IQueryCollection queryParameters)
    {
        var collection = new NameValueCollection();

        var parameters = queryParameters.ToArray();

        for (int i = 0; i < parameters.Length; i++)
        {
            var query = parameters[i];

            collection.Add(query.Key, query.Value);
        }

        return collection.GetCanonicalQueryParameters();
    }

    /// <summary>
    ///     Retrieves HTTP request body content as string
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Body content as <see cref="string"/></returns>
    public static async ValueTask<string> GetRequestBodyAsStringAsync(this HttpRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        var body = await request.Body.GetStringAsync(cancellationToken);

        return body ?? string.Empty;
    }

    /// <summary>
    ///     Converts <see cref="NameValueCollection"/> query parameters to canonical string
    /// </summary>
    /// <param name="queryParameters">Query parameters</param>
    /// <returns>Canonical string</returns>
    public static ReadOnlySpan<char> GetCanonicalQueryParameters(this NameValueCollection queryParameters)
    {
        var canonicalQuery = string.Join('&', queryParameters.AllKeys.Select(key => $"{key!.UrlEncode()}={queryParameters[key]!.UrlEncode()}")).AsSpan();

        return canonicalQuery;
    }

    /// <summary>
    ///     Converts request headers to canonical string
    /// </summary>
    /// <param name="headers">Request headers</param>
    /// <param name="signedHeaders">Collection of headers that where signed (accessible headers are defined at <see cref="HTTPHeaders.SignedHeaders"/>)</param>
    /// <returns>Canonical string</returns>
    public static ReadOnlySpan<char> GetCanonicalHeaders(this IDictionary<string, string> headers, out string[] signedHeaders)
    {
        signedHeaders = Array.Empty<string>();

        var sortedHeaders = new SortedDictionary<string, string>(headers);
        var stringBuilder = new StringBuilder();

        var possiblySignedHeaders = sortedHeaders.Where(header => HttpExtensionsConstants.CanonicalHeaders.Contains(header.Key)).ToArray();

        for (int i = 0; i < possiblySignedHeaders.Length; i++)
        {
            var header = possiblySignedHeaders[i];

            stringBuilder.AppendFormat("{0}:{1}\n", header.Key, header.Value);
            signedHeaders = signedHeaders.Append(header.Key).ToArray();
        }

        return stringBuilder.ToString().AsSpan();
    }
}