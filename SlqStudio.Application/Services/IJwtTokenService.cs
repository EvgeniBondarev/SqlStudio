namespace SlqStudio.Application.Services;

﻿public interface IJwtTokenService
{
    string GenerateJwtToken(string email, string role);
    bool ValidateJwtToken(string authToken);
}