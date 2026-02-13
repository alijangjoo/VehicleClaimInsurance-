namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;

public interface ITokenService
{
    Task<TokenResult> GenerateTokenAsync(AppUser user, IEnumerable<OrganizationPersona> personas);
    Task<TokenResult> GenerateTokenWithPersonaAsync(AppUser user, BusinessKey personaKey);
    Task<TokenResult> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    Task<BusinessKey?> GetCurrentPersonaFromTokenAsync(string token);
}