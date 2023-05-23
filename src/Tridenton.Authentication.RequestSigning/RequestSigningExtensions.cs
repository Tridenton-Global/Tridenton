using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Tridenton.Extensions.Http;

namespace Tridenton.Authentication.RequestSigning;

public static class RequestSigningExtensions
{
    public static IServiceCollection AddRequestSigning(this IServiceCollection services, Action<RequestSigningOptionsBuilder>? options = null)
    {
        var settings = new RequestSigningOptionsBuilder();

        options?.Invoke(settings);

        ArgumentNullException.ThrowIfNull(settings.VerifierType, $"Verifier was not provided");

        Type? signerType = null;

        if (settings.Algorithm == SigningAlgorithm.HmacSHA256) signerType = typeof(HMAC256Signer);
        else throw new InvalidEnumArgumentException("Specified signing algorithm is invalid");

        services.AddTransient(typeof(IRequestSigner), signerType);
        services.AddTransient(typeof(RequestSignatureVerifier), settings.VerifierType);

        return services;
    }

    /// <summary>
    ///     Converts authorization header value to RequestSignature model
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <param name="requestSignature">Request signature model with algorithm, credential and signature</param>
    /// <returns>
    /// <see langword="true"/> if authorization header is present and valid, otherwise - <see langword="false"/>
    /// </returns>
    public static bool TryRetrieveSignature(this HttpRequest request, out RequestSignature requestSignature)
    {
        requestSignature = RequestSignature.Empty;

        try
        {
            if (!request.IsRequestHeaderPresent(HeaderNames.Host, out var authorization)) return false;

            var segments = authorization.Split(" ");

            if (segments.Length != 3) return false;

            var algorithm = Enumeration.GetValue<SigningAlgorithm>(segments[0]);
            if (algorithm is null) throw new ArgumentException("Signing algorithm is invalid");

            var credential = segments[1].Split("=")[1];
            var signature = segments[2].Split("=")[1];

            requestSignature = new RequestSignature(algorithm, credential.Replace(",", string.Empty), signature);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}