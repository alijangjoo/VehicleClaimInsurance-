using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OysterFx.AppCore.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly InsuranceUserServiceDbContext _context;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        InsuranceUserServiceDbContext context,
        ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _logger = logger;
    }

    public async Task<TokenResult> GenerateTokenAsync(AppUser user, IEnumerable<OrganizationPersona> personas)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.UserName ?? ""),
            //new Claim("businessKey", user.BusinessKey.Value)
        };

        // اضافه کردن Personaها به claims
        if (personas != null && personas.Any())
        {
            var defaultPersona = personas.First();
            //claims.Add(new Claim("personaKey", defaultPersona.BusinessKey.Value));
            //claims.Add(new Claim("personaName", defaultPersona.Name));
            //claims.Add(new Claim("organizationKey", defaultPersona.OrganizationBusinessKey.Value));

            // ذخیره همه Personaها در یک claim
            var personasJson = System.Text.Json.JsonSerializer.Serialize(
                personas.Select(p => new { p.BusinessKey.Value, p.Name }));
            claims.Add(new Claim("personas", personasJson));
        }

        // اضافه کردن نقش‌ها
        //var roles = await _context.UserRoles
        //    .Where(ur => ur.UserId == user.BusinessKey)
        //    .Join(_context.Roles,
        //        ur => ur.RoleId,
        //        r => r.BusinessKey,
        //        (ur, r) => r.Name)
        //    .ToListAsync();

        //foreach (var role in roles)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, role ?? ""));
        //}

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expires
        };
    }

    public async Task<TokenResult> GenerateTokenWithPersonaAsync(AppUser user, BusinessKey personaKey)
    {
        var persona = await _context.Personas
            .FirstOrDefaultAsync(p => p.BusinessKey == personaKey);

        if (persona == null)
            throw new InvalidOperationException("Persona not found");

        return await GenerateTokenAsync(user, new[] { persona });
    }

    private async Task<string> GenerateRefreshTokenAsync(long userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        // ذخیره در دیتابیس (شما باید یک DbSet<RefreshToken> به Context اضافه کنید)
        await _context.Set<RefreshToken>().AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<TokenResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromToken(accessToken);
        if (principal == null)
            throw new SecurityTokenException("Invalid access token");

        var userId = long.Parse(principal.FindFirst("userId")?.Value ?? "0");

        // بررسی Refresh Token
        var storedRefreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null ||
            storedRefreshToken.IsRevoked ||
            storedRefreshToken.ExpirationDate < DateTime.UtcNow)
            throw new SecurityTokenException("Invalid refresh token");

        // دریافت کاربر
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new SecurityTokenException("User not found");

        // دریافت Personaهای کاربر
        var personas = await _context.UserPersonas
            .Where(up => up.UserBusinessKey == user.BusinessKey)
            .Select(up => up.Persona)
            .ToListAsync();

        // Revoke old refresh token
        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.ReasonRevoked = "Replaced by new token";
        await _context.SaveChangesAsync();

        // Generate new tokens
        return await GenerateTokenAsync(user, personas);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedRefreshToken == null)
            return false;

        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.ReasonRevoked = "Manually revoked";
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
                return false;

            var userId = principal.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _context.Users.FindAsync(long.Parse(userId));
            return user != null;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return null;
        }
    }

    public async Task<BusinessKey?> GetCurrentPersonaFromTokenAsync(string token)
    {
        var principal = GetPrincipalFromToken(token);
        var personaKeyValue = principal?.FindFirst("personaKey")?.Value;

        if (string.IsNullOrEmpty(personaKeyValue))
            return null;

        return BusinessKey.FromGuid(Guid.Parse(personaKeyValue));
    }
}