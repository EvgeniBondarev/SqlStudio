using System.Security.Claims;

namespace SlqStudio.Application.Services;

public interface IJwtTokenHandler
{
    string GetEmailFromToken(string token);
    string GetEmailFromClaims(ClaimsPrincipal claimsPrincipal);
}