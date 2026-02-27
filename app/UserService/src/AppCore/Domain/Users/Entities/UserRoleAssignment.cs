namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class UserRoleAssignment : Entity<long>
{
    public BusinessKey RoleBusinessKey { get; private set; }
    public BusinessKey UserMembershipBusinessKey { get; private set; }

    private UserRoleAssignment() { }

    internal static UserRoleAssignment Create(
        BusinessKey membershipBusinessKey,
        BusinessKey roleBusinessKey)
    {
        return new UserRoleAssignment
        {
            UserMembershipBusinessKey = membershipBusinessKey,
            RoleBusinessKey = roleBusinessKey
        };
    }
}