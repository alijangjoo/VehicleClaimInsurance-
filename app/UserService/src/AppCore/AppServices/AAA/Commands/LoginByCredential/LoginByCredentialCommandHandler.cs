namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByCredential;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Threading.Tasks;

public class LoginByCredentialCommandHandler : CommandHandler<LoginByCredentialCommand, LoginByCredentialCommandResult?>
{
    private readonly ICacheServiceCaller _cacheServiceCaller;

    public LoginByCredentialCommandHandler(ICacheServiceCaller cacheServiceCaller)
    {
        _cacheServiceCaller = cacheServiceCaller;
    }

    public override async Task<CommandResult<LoginByCredentialCommandResult?>> Handle(LoginByCredentialCommand command)
    {
        await _cacheServiceCaller.SetAsync(new(Key: $"user--permissions", Value: ""));
        return default;
    }
}