using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SharedKernel.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET") 
                     ?? "SuperSecretKeyThatIsAtLeast256BitsLong!!12345";
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TicketApi";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TicketApi";
        var expiryMinutesStr = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60";
        
        int.TryParse(expiryMinutesStr, out var expiryMinutes);
        if (expiryMinutes <= 0)
        {
            expiryMinutes = 60;
        }

        var jwtSettings = new JwtSettings
        {
            Secret = secret,
            Issuer = issuer,
            Audience = audience,
            ExpiryMinutes = expiryMinutes
        };

        services.AddSingleton(jwtSettings);
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}
