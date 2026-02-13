namespace Insurance.UserService.AppCore.Domain.Organizations.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public class OrganizationPersona : Entity<long>
{
    public BusinessKey UserBusinessKey { get; private set; }
    public BusinessKey OrganizationBusinessKey { get; private set; }
    public string Name { get; private set; }

    private OrganizationPersona() { }

    private OrganizationPersona(BusinessKey userBusinessKey, BusinessKey organizationBusinessKey, string name)
    {
        UserBusinessKey = userBusinessKey;
        OrganizationBusinessKey = organizationBusinessKey;
        Name = name;
    }

    public static OrganizationPersona NewPersona(BusinessKey userBusinessKey, BusinessKey organizationBusinessKey, string name)
        => new OrganizationPersona(userBusinessKey, organizationBusinessKey, name);
}