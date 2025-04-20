using SlqStudio.Application.Services;
using SlqStudio.Application.Services.Implementation;
using SlqStudio.Application.Services.Models;

namespace SlqStudio.Extensions;

public static class JwtTokenExtensions
{
    public static void AddJwtTokenServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings
        {
            Key = configuration["Jwt:SecretKey"],
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            ExpirationMinutes = 30
        };

        services.AddSingleton(jwtSettings);
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();
    }
}