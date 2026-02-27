namespace Insurance.UserService.AppCore.Shared.AAA.Services;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;

public interface ITokenService
{
    /// <summary>
    /// ایجاد توکن برای کاربر با تمام سازمان‌ها و نقش‌هایش (UserMemberships)
    /// </summary>
    Task<TokenResult> GenerateTokenAsync(AppUser user, IEnumerable<UserMembership> memberships);

    /// <summary>
    /// ایجاد توکن برای کاربر با یک سازمان و نقش مشخص (MembershipKey)
    /// </summary>
    Task<TokenResult> GenerateTokenWithMembershipAsync(AppUser user, BusinessKey membershipKey);

    /// <summary>
    /// نوسازی توکن با توکن دسترسی و RefreshToken
    /// </summary>
    Task<TokenResult> RefreshTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    /// ابطال RefreshToken
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken);

    /// <summary>
    /// اعتبارسنجی یک Access Token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// گرفتن ClaimsPrincipal از توکن
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// گرفتن BusinessKey Membership فعلی از توکن
    /// </summary>
    Task<BusinessKey?> GetCurrentMembershipFromTokenAsync(string token);
}