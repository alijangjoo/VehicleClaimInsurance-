namespace Insurance.UserService.AppCore.AppServices.Users.Commands.AssignRoleToMembership;

using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.AssignRoleToMembership;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Linq;
using System.Threading.Tasks;

public class AssignRoleToMembershipCommandHandler : CommandHandler<AssignRoleToMembershipCommand, bool>
{
    private readonly IUserCommandRepository _userCommandRepository;

    public AssignRoleToMembershipCommandHandler(IUserCommandRepository userCommandRepository)
    {
        _userCommandRepository = userCommandRepository;
    }

    public override async Task<CommandResult<bool>> Handle(AssignRoleToMembershipCommand command)
    {
        var user = await _userCommandRepository.GetAsync(BusinessKey.FromGuid(command.UserBusinessKey));

        if (user is null)
            return CommandResult.Failure("User not found");

        var membership = user.Memberships.FirstOrDefault(x => x.BusinessKey == BusinessKey.FromGuid(command.MembershipBusinessKey));

        if (membership is null)
            return CommandResult.Failure("Membership not found");

        membership.AssignRole(
            BusinessKey.FromGuid(command.RoleBusinessKey));

        await _userCommandRepository.CommitAsync();

        return await OkAsync(true);
    }
}