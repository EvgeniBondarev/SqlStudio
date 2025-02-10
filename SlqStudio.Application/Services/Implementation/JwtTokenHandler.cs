using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SlqStudio.Application.Services.Implementation;

public class JwtTokenHandler : IJwtTokenHandler
{
    public string GetEmailFromToken(string token)
    {
        var validatedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return validatedToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
    }

    public string GetEmailFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var claims = claimsPrincipal.Identities.FirstOrDefault()?.Claims;
        return claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    }
}