namespace Tridenton.Authentication.RequestSigning;

public sealed class RequestSigningOptionsBuilder
{
    internal Type? VerifierType { get; private set; }

    internal SigningAlgorithm Algorithm { get; private set; }

    internal RequestSigningOptionsBuilder()
    {
        Algorithm = SigningAlgorithm.HmacSHA256;
    }

    /// <summary>
    ///		Specifies a signing algorithm. By default - <see cref="SigningAlgorithm.HmacSHA256"/>
    /// </summary>
    /// <param name="algorithm"></param>
    public RequestSigningOptionsBuilder WithAlgorithm(SigningAlgorithm algorithm)
    {
        Algorithm = algorithm;
        return this;
    }

    /// <summary>
    ///		Specifies a <see cref="RequestSignatureVerifier"/> implementation to use for signing verification.
    ///		This method is required to be called; otherwise, an <see cref="ArgumentNullException"/> will be thrown during configuration
    /// </summary>
    /// <typeparam name="TVerifier"></typeparam>
    public RequestSigningOptionsBuilder WithVerifier<TVerifier>() where TVerifier : RequestSignatureVerifier
    {
        if (VerifierType is null)
        {
            VerifierType = typeof(TVerifier);
        }

        return this;
    }
}