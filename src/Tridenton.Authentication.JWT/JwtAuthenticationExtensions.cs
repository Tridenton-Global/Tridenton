using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Tridenton.Authentication.JWT;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, Action<JwtAuthenticationOptionsBuilder> options)
    {
        var settings = new JwtAuthenticationOptionsBuilder();
        options.Invoke(settings);

        if (settings.TokenValidationParameters is null)
        {
            settings.TokenValidationParameters = new();
        }

        settings.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        services
            .AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg => cfg = settings);

        return services;
    }
}