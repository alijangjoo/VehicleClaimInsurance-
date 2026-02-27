namespace Insurance.UserService.AppCore.Shared.Users.Commands;

using Insurance.UserService.AppCore.Domain.Users.Entities;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Threading.Tasks;

public interface IUserCommandRepository
{
    AppUser Get(BusinessKey BusinessKey);
    Task<AppUser> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(AppUser entity);
    Task<int> CommitAsync();
}