namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class UserRole : Entity<long>
{
    public BusinessKey UserId { get; private set; }
    public BusinessKey RoleId { get; private set; }
    public BusinessKey PersonaId { get; private set; }

    private UserRole() { }

    private UserRole(BusinessKey userId, BusinessKey roleId, BusinessKey personaId)
    {
        UserId = userId;
        RoleId = roleId;
        PersonaId = personaId;
    }

    public static UserRole AssignUserRole(BusinessKey userId, BusinessKey roleId, BusinessKey personaId)
        => new(userId, roleId, personaId);
}
