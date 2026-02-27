namespace Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateUserCommand : ICommand<Guid>
{
    public string? MobileNumber { get; set; }
    public string? UserName { get; set; }
}