namespace SlqStudio.Application.Services;

﻿public interface IJwtTokenService
{
    string GenerateJwtToken(string email, string role, string fullName);
    bool ValidateJwtToken(string authToken);
}