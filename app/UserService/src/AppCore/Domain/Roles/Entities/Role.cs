namespace Insurance.UserService.AppCore.Domain.Roles.Entities;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class AppRole : IdentityRole<long>
{
    public BusinessKey BusinessKey { get; private set; }
    public RoleScope Scope { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private AppRole() { }

    private AppRole(string name, RoleScope scope)
    {
        Name = name.Trim();
        Scope = scope;
        BusinessKey = BusinessKey.FromGuid(Guid.NewGuid());
    }

    public static AppRole NewRole(string name, RoleScope scope) => new(name, scope);
}