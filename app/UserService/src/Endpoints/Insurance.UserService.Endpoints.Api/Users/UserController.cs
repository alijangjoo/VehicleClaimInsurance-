namespace Insurance.UserService.Endpoints.Api.Users;

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
}