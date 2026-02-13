namespace OysterFx.Infra.Auth.JwtServices;

using OysterFx.Infra.Auth.UserServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class JwtUserInfoService : IUserInfoService
{

    public string? UserId { get; private set; }
    public string? Username { get; private set; }

    public string? GetClaim(string claimType) => string.Empty;

    public string GetFirstName() => string.Empty;

    public string GetLastName() => string.Empty;

    public string GetUserAgent() => string.Empty;

    public string GetUserId() => string.Empty;

    public string GetUserIp() => string.Empty;

    public string GetUsername() => string.Empty;

    public bool IsCurrentUser(string userId) => userId == UserId;

    public void LoadFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                ResetUserInfo();
                return;
            }

            // Clean the token - remove any extra spaces or quotes
            token = token.Trim().Replace("\"", "");

            var handler = new JwtSecurityTokenHandler();

            // Check if token is valid
            if (!handler.CanReadToken(token))
            {
                ResetUserInfo();
                return;
            }

            // Read the token
            var jwtToken = handler.ReadJwtToken(token);
            ExtractUserInfoFromToken(jwtToken);
        }
        catch (Exception ex)
        {
            ResetUserInfo();
        }
    }

    private void ExtractUserInfoFromToken(JwtSecurityToken jwtToken)
    {
        // Try multiple claim types for user ID
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == "UserId" ||
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        UserId = userIdClaim.Value;
        // Try multiple claim types for username
        var usernameClaim = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.Name ||
            c.Type == "username" ||
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        Username = usernameClaim?.Value;
    }

    private void ResetUserInfo()
    {
        UserId = null;
        Username = null;
    }

    public string UserIdOrDefault() => UserId!;

    public string UserIdOrDefault(string defaultValue) => !string.IsNullOrWhiteSpace(UserId) ? UserId : default!;
}
