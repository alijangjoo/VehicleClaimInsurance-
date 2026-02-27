namespace Insurance.UserService.AppCore.Domain.Common;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public AppUser? User { get; set; }

    /// <summary>
    /// عضویت‌های کاربر در سازمان‌ها و نقش‌ها
    /// </summary>
    public IEnumerable<UserMembership>? Memberships { get; set; }

    public DateTime TokenExpiration { get; set; }

    public static AuthResult Success(string token, AppUser user, IEnumerable<UserMembership> memberships)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Token = token,
            User = user,
            Memberships = memberships,
            TokenExpiration = DateTime.UtcNow.AddHours(8) // مثلاً 8 ساعت
        };
    }

    public static AuthResult Success(string token, AppUser user, IEnumerable<UserMembership> memberships, DateTime expiration)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Token = token,
            User = user,
            Memberships = memberships,
            TokenExpiration = expiration
        };
    }

    public static AuthResult Failed(string message)
    {
        return new AuthResult
        {
            IsSuccess = false,
            Message = message
        };
    }

    public static AuthResult Failed(string message, Exception ex)
    {
        return new AuthResult
        {
            IsSuccess = false,
            Message = $"{message}: {ex.Message}"
        };
    }
}