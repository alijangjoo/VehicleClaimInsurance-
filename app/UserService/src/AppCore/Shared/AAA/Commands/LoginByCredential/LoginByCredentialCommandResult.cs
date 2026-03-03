namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public class LoginByCredentialCommandResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public Account User { get; set; } = null!;
    public List<MembershipDto> Memberships { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

public class MembershipDto
{
    public BusinessKey BusinessKey { get; set; }
    public BusinessKey RoleBusinessKey { get; set; }
    public BusinessKey OrganizationBusinessKey { get; set; }
}