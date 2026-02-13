namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class UserPersona : Entity<long>
{
    public BusinessKey UserBusinessKey { get; private set; }
    public BusinessKey PersonaBusinessKey { get; private set; }

    public AppUser User { get; private set; } = null!;
    public OrganizationPersona Persona { get; private set; } = null!;

    private UserPersona() { }

    public UserPersona(BusinessKey userBusinessKey, BusinessKey personaBusinessKey)
    {
        UserBusinessKey = userBusinessKey;
        PersonaBusinessKey = personaBusinessKey;
    }

    public static UserPersona AssignUserPersona(BusinessKey userBusinessKey, BusinessKey personaBusinessKey)
        => new(userBusinessKey, personaBusinessKey);
}