namespace Insurance.UserService.AppCore.AppServices.Users.Commands.CreateUser;

using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserCommandRepository _userCommandRepository;

    public CreateUserCommandHandler(IUserCommandRepository userCommandRepository)
    {
        _userCommandRepository = userCommandRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(
        CreateUserCommand command)
    {
        var user = AppUser.NewUser(
            command.MobileNumber!,
            command.UserName!);

        await _userCommandRepository.InsertAsync(user);
        await _userCommandRepository.CommitAsync();

        return await OkAsync(user.BusinessKey.Value);
    }
}