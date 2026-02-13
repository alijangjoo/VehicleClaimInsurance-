namespace Insurance.UserService.AppCore.Domain.Users.Dtos;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public class UserContext
{
    public long UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public BusinessKey UserBusinessKey { get; set; } = null!;
    public BusinessKey? CurrentPersonaKey { get; set; }
    public OrganizationPersona? CurrentPersona { get; set; }
    public List<OrganizationPersona> AvailablePersonas { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public Organization? CurrentOrganization { get; set; }
    public List<Organization> Organizations { get; set; } = new();
    public bool IsAuthenticated { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public bool HasPermission(string permissionCode)
    {
        return Permissions.Contains(permissionCode);
    }

    public bool HasAnyPermission(params string[] permissionCodes)
    {
        return permissionCodes.Any(p => Permissions.Contains(p));
    }

    public bool HasAllPermissions(params string[] permissionCodes)
    {
        return permissionCodes.All(p => Permissions.Contains(p));
    }
}