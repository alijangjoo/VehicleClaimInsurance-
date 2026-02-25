namespace Insurance.CoreService.Endpoints.Api.Losts;

using Insurance.AppCore.Shared.Losts.Commands.AssignHullThirdPartyInsurerCode;
using Insurance.AppCore.Shared.Losts.Commands.AssignThirdPartyInsurerCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;
using System.Reflection.Metadata.Ecma335;

[Authorize]
[Route("api/CoreService/[controller]")]
public class LostController : OysterFxController
{
    [Authorize]
    [HttpPost("AssignThirdPartyInsurerCode")]
    public async Task<IActionResult> AssignThirdPartyInsurerCode([FromBody] AssignThirdPartyInsurerCodeCommand command)
        => await SendCommand<AssignThirdPartyInsurerCodeCommand, Guid>(command);

    [Authorize]
    [HttpPost("AssignHullThirdPartyInsurerCode")]
    public async Task<IActionResult> AssignHullThirdPartyInsurerCode([FromBody] AssignHullThirdPartyInsurerCodeCommand command)
        => await SendCommand<AssignHullThirdPartyInsurerCodeCommand, Guid>(command);

    [AllowAnonymous]
    //[Authorize]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        return Ok("Empty get List");
    }

    [AllowAnonymous]
    //[Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create()
    {
        return Ok("Empty post");
    }
}