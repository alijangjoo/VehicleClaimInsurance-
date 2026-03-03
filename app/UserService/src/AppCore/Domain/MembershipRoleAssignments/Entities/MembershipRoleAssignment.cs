namespace Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class MembershipRoleAssignment : AggregateRoot
{
    public BusinessKey RoleBusinessKey { get; private set; }
    public BusinessKey MembershipBusinessKey { get; private set; }

    private MembershipRoleAssignment() { }

    internal static MembershipRoleAssignment Create(
        BusinessKey membershipBusinessKey,
        BusinessKey roleBusinessKey)
    {
        return new MembershipRoleAssignment
        {
            MembershipBusinessKey = membershipBusinessKey,
            RoleBusinessKey = roleBusinessKey
        };
    }
}