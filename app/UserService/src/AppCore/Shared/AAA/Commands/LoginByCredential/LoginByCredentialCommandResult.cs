namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

using Insurance.UserService.AppCore.Domain.Users.Entities;

public class LoginByCredentialCommandResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public AppUser User { get; set; } = null!;
    public List<UserMembership> Memberships { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}