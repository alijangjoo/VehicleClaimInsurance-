namespace Insurance.UserService.AppCore.Shared.Organizations.Commands;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IOrganizationCommandRepository : ICommandRepository<Organization, long>
{
}