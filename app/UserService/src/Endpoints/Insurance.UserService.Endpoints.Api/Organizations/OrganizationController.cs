namespace Insurance.UserService.Endpoints.Api.Organizations;

using Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

// [Authorize]
[AllowAnonymous]
[ApiController]
[Route("api/UserService/[controller]")]

public class OrganizationController : OysterFxController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand command)
        => await SendCommand<CreateOrganizationCommand, Guid>(command);
}