namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;

public sealed class UserMembership : Entity<long>
{
    public BusinessKey BusinessKey { get; private set; }
    public TenantId TenantId { get; private set; }
    public BusinessKey UserBusinessKey { get; private set; }
    public BusinessKey OrganizationBusinessKey { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public bool IsActive { get; private set; }

    private readonly List<UserRoleAssignment> _roleAssignments = new();
    public IReadOnlyCollection<UserRoleAssignment> RoleAssignments => _roleAssignments.AsReadOnly();

    private UserMembership() { }

    internal static UserMembership Create(
        BusinessKey userBusinessKey,
        TenantId tenantId,
        BusinessKey organizationBusinessKey,
        DateTime startDate)
    {
        return new UserMembership
        {
            BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()),
            UserBusinessKey = userBusinessKey,
            TenantId = tenantId,
            OrganizationBusinessKey = organizationBusinessKey,
            StartDate = startDate,
            IsActive = true
        };
    }

    public void AssignRole(BusinessKey roleBusinessKey)
    {
        if (!IsActive)
            throw new AggregateStateExceptions("Cannot assign role to inactive membership.", "");

        if (_roleAssignments.Any(x => x.RoleBusinessKey == roleBusinessKey))
            return;

        var roleAssignment = UserRoleAssignment.Create(
            this.BusinessKey,
            roleBusinessKey);

        _roleAssignments.Add(roleAssignment);
    }

    public void RemoveRole(BusinessKey roleBusinessKey)
    {
        var existing = _roleAssignments
            .FirstOrDefault(x => x.RoleBusinessKey == roleBusinessKey);

        if (existing != null)
            _roleAssignments.Remove(existing);
    }

    public void Deactivate()
    {
        IsActive = false;
        EndDate = DateTime.UtcNow;
    }
}