namespace Insurance.UserService.Endpoints.Api.Permissions;

using Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateApplicationPermission;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateOrganizationPermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/UserService/[controller]")]
public class PermissionController : OysterFxController
{
    [Authorize]
    [HttpPost("system")]
    public async Task<IActionResult> CreateSystemPermission([FromBody] CreateSystemPermissionCommand command)
        => await SendCommand<CreateSystemPermissionCommand, Guid>(command);

    [Authorize]
    [HttpPost("organization")]
    public async Task<IActionResult> CreateOrganizationPermission([FromBody] CreateOrganizationPermissionCommand command)
        => await SendCommand<CreateOrganizationPermissionCommand, Guid>(command);

    [Authorize]
    [HttpPost("application")]
    public async Task<IActionResult> CreateApplicationPermission([FromBody] CreateApplicationPermissionCommand command)
        => await SendCommand<CreateApplicationPermissionCommand, Guid>(command);
}