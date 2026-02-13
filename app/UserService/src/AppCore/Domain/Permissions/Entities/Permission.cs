namespace Insurance.UserService.AppCore.Domain.Permissions.Entities;

using Insurance.UserService.AppCore.Domain.Roles.Enums;
using OysterFx.AppCore.Domain.Aggregates;

public class Permission : Entity<long>
{
    public string Code { get; private set; }
    public RoleScope Scope { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Permission() { }

    private Permission(string code, RoleScope scope, string? description = null)
    {
        Code = code.Trim().ToUpperInvariant(); ;
        Scope = scope;
        Description = description;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}