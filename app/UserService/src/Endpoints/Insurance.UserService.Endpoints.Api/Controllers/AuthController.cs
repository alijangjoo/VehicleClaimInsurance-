namespace Insurance.UserService.Endpoints.Api.Controllers;

using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;
using Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;
using Insurance.UserService.Endpoints.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/UserService/[controller]")]
public class AuthController : OysterFxController
{
    private readonly UserManager<AppUser> _userManager;

    public AuthController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existing = await _userManager.Users.FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber);

        if (existing != null)
            return BadRequest("Mobile number already exists.");

        var user = AppUser.NewUser(request.MobileNumber, request.MobileNumber);

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new UserIdResponse(user.Id.ToString()));
    }

    [HttpGet("by-mobile/{mobileNumber}")]
    public async Task<IActionResult> GetUserIdByMobile(string mobileNumber)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);

        if (user == null)
            return NotFound("User not found.");

        return Ok(new UserIdResponse(user.Id.ToString()));
    }

    [HttpPost("login/otp")]
    public async Task<IActionResult> LoginByOtp([FromBody] LoginByOtpCommand request)
       => await SendCommand<LoginByOtpCommand, LoginByOtpCommandResult?>(LoginByOtpCommand.CreateInstance(request.MobileNumber));

    [HttpPost("login/otp/{mobileNumber}/verify")]
    public async Task<IActionResult> VerifyOtpLogin([FromRoute] string mobileNumber, [FromBody] string? code)
        => await SendCommand<VerifyLoginOtpCommand, VerifyLoginOtpCommandResult?>(VerifyLoginOtpCommand.CreateInstance(mobileNumber, code));
}