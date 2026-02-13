namespace Insurance.UserService.AppCore.Domain.Organizations.Entities;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public class Organization : AggregateRoot
{
    public string Name { get; private set; }
    public bool IsActive { get; private set; }


    private readonly List<OrganizationPersona> _personas = new();
    public IReadOnlyCollection<OrganizationPersona> Personas => _personas.AsReadOnly();

    private Organization() { }

    private Organization(string name)
    {
        Name = name;
        IsActive = true;
    }

    public static Organization NewOrganization(string name) => new(name);

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public OrganizationPersona CreatePersona(BusinessKey userBusinessKey, string name)
    {
        var persona = OrganizationPersona.NewPersona(userBusinessKey, BusinessKey, name);
        _personas.Add(persona);
        return persona;
    }
}