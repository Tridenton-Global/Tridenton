using Microsoft.Net.Http.Headers;

namespace Tridenton.Extensions.Http;

public readonly struct HttpExtensionsConstants
{
    internal static readonly string[] CanonicalHeaders = new[]
    {
        HeaderNames.Accept,
        HeaderNames.AcceptEncoding,
        HeaderNames.ContentEncoding,
        HeaderNames.ContentLanguage,
        HeaderNames.ContentLength,
        HeaderNames.ContentMD5,
        HeaderNames.ContentType,
        HeaderNames.Date,
        HeaderNames.Host
    };
}