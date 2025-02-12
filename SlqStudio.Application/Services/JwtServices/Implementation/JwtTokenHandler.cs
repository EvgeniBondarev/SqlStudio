﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SlqStudio.Application.Services.Models;

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
    public (string Email, UserRole Role, string name) GetClaimsFromToken(string token)
    {
        var validatedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var email = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var roleString = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        var name = validatedToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        Enum.TryParse(roleString, true, out UserRole role);
        return (email, role, name);
    }
}