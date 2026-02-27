namespace Insurance.UserService.AppCore.Shared.Permissions.Commands;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IPermissionCommandRepository : ICommandRepository<Permission, long>
{
}