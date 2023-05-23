using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Tridenton.Extensions.Http;

namespace Tridenton.Authentication.RequestSigning;

public record RequestVerificationResponse(SigningCredentials Credentials, string RequestBody);

public abstract class RequestSignatureVerifier
{
    protected readonly IRequestSigner Signer;

    public RequestSignatureVerifier(IRequestSigner signer)
    {
        Signer = signer;
    }

    /// <summary>
    ///     Verifies request signature
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <param name="cancellationToken">Cancellation token (optional)</param>
    /// <returns><see cref="RequestVerificationResponse"/></returns>
    /// <exception cref="InvalidAuthorizationHeaderException"></exception>
    /// <exception cref="SignatureDoesNotMatchException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public virtual async ValueTask<RequestVerificationResponse> VerifyRequestSignAsync(HttpRequest request, CancellationToken cancellationToken = default)
    {
        var isAuthorizationValid = request.TryRetrieveSignature(out var signature);

        if (!isAuthorizationValid) throw new InvalidAuthorizationHeaderException();

        request.Headers.Remove(HeaderNames.Authorization);

        var credentials = await GetCredentialsAsync(signature, cancellationToken);

        var secretAccessKey = credentials.SecretKey;

        var canonicalRequest = await GetCanonicalRequestAsync(request, cancellationToken);
        var stringToSign = await Signer.CreateStringToSignAsync(canonicalRequest.ToString(), cancellationToken);
        var signatureCalculated = await Signer.ComputeSignatureAsync(stringToSign, secretAccessKey, cancellationToken);

        if (!signature.Signature.Equals(signatureCalculated, StringComparison.Ordinal)) throw new SignatureDoesNotMatchException();

        return new(credentials, canonicalRequest.PayloadHash.Payload);
    }

    private async ValueTask<CanonicalRequest> GetCanonicalRequestAsync(HttpRequest request, CancellationToken cancellationToken = default)
    {
        return new(
            request.Method,
            request.Path.Value!,
            request.Query.GetCanonicalQueryParameters().ToString(),
            GetCanonicalHeaders(request, out var signedHeaders).ToString(),
            string.Join(";", signedHeaders),
            await GetPayloadHashAsync(request, cancellationToken));
    }

    private static ReadOnlySpan<char> GetCanonicalHeaders(HttpRequest request, out string[] signedHeaders)
    {
        signedHeaders = Array.Empty<string>();

        var allHeaders = request.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value!));

        return allHeaders.GetCanonicalHeaders(out signedHeaders);
    }

    private async ValueTask<PayloadHash> GetPayloadHashAsync(HttpRequest request, CancellationToken cancellationToken = default)
    {
        var payload = await request.GetRequestBodyAsStringAsync(cancellationToken);

        var payloadHash = await Signer.ComputeHashAsync(payload, cancellationToken);

        return new(payload, payloadHash.ToHexString().ToString());
    }

    protected abstract ValueTask<SigningCredentials> GetCredentialsAsync(RequestSignature signature, CancellationToken cancellationToken = default);
}

record CanonicalRequest(string Method, string Path, string CanonicalQueryParameters, string CanonicalHeaders, string SignedHeaders, PayloadHash PayloadHash)
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(Method);
        stringBuilder.AppendLine(Path);
        stringBuilder.AppendLine(CanonicalQueryParameters);
        stringBuilder.AppendLine(CanonicalHeaders);
        stringBuilder.AppendLine(SignedHeaders);
        stringBuilder.Append(PayloadHash.PayloadHashHex);

        return stringBuilder.ToString();
    }
}

record PayloadHash(string Payload, string PayloadHashHex);