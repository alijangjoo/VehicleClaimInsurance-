namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

public class LoginByCredentialCommandResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}