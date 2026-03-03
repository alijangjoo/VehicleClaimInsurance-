namespace Insurance.UserService.Endpoints.Api.Users;

using Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;
using Insurance.UserService.AppCore.Shared.Users.Commands.AssignRoleToMembership;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

// [Authorize]
[AllowAnonymous]
[ApiController]
[Route("api/UserService/[controller]")]
public class UserController : OysterFxController
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command) 
        => await SendCommand<CreateUserCommand, Guid>(command);

    [HttpPost("add-membership")]
    public async Task<IActionResult> AddMembership([FromBody] AddMembershipCommand command)
        => await SendCommand<AddMembershipCommand, bool>(command);

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleToMembershipCommand command)
        => await SendCommand<AssignRoleToMembershipCommand, bool>(command);
}