namespace Insurance.UserService.AppCore.AppServices.Users.Commands.AddMembership;

using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class AddMembershipCommandHandler : CommandHandler<AddMembershipCommand, bool>
{
    private readonly IUserCommandRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public AddMembershipCommandHandler(IUserCommandRepository userRepository, IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _userContextService = userContextService;
    }

    public override async Task<CommandResult<bool>> Handle(AddMembershipCommand command)
    {
        var user = await _userRepository.GetAsync(BusinessKey.FromGuid(command.UserBusinessKey));

        if (user is null)
            return CommandResult.Failure("User not found");

        //user.AddMembership(
        //    _userContextService.GetCurrentUserContextAsync().Result!.TenantId,
        //    BusinessKey.FromGuid(command.OrganizationBusinessKey),
        //    DateTime.UtcNow);

        await _userRepository.CommitAsync();

        return await OkAsync(true);
    }
}