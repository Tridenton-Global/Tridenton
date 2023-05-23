using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Tridenton.Authentication.JWT;

public sealed class JwtAuthenticationOptionsBuilder : JwtBearerOptions
{
    internal string Key { get; private set; }

    public JwtAuthenticationOptionsBuilder()
    {
        Key = string.Empty;
    }

    public JwtAuthenticationOptionsBuilder WithKey(string key)
    {
        Key = key;
        return this;
    }
}