namespace Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;

public class VerifyLoginOtpCommandResult
{
    public bool Verified { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}