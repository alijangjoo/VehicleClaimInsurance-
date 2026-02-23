namespace Insurance.UserService.AppCore.Domain.Roles.Entities;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class RolePermission : Entity<long>
{
    public BusinessKey RoleBusinessKey { get; private set; }
    public BusinessKey PermissionBusinessKey { get; private set; }

    private RolePermission() { }

    private RolePermission(BusinessKey roleBusinessKey, BusinessKey permissionBusinessKey)
    {
        RoleBusinessKey = roleBusinessKey;
        PermissionBusinessKey = permissionBusinessKey;
    }

    public static RolePermission AssignRolePermission(BusinessKey roleBusinessKey, BusinessKey permissionBusinessKey)
        => new(roleBusinessKey, permissionBusinessKey);
}