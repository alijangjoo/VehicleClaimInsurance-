namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByOtp;

using Insurance.PanelPayamakService.Infra.Abstractions;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.DomainServices;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class LoginByOtpCommandHandler : CommandHandler<LoginByOtpCommand, LoginByOtpCommandResult?>
{
    private readonly IPayamakSender _payamakSender;
    private readonly IOtpGeneratorDomainService _otpGeneratorDomainService;

    public LoginByOtpCommandHandler(
        IPayamakSender payamakSender,
        IOtpGeneratorDomainService otpGeneratorDomainService)
    {
        _payamakSender = payamakSender;
        _otpGeneratorDomainService = otpGeneratorDomainService;
    }

    public override async Task<CommandResult<LoginByOtpCommandResult?>> Handle(LoginByOtpCommand command)
    {
        CommandResult<LoginByOtpCommandResult?> commandResult = CommandResult<LoginByOtpCommandResult?>.Default();

        var code = await _otpGeneratorDomainService.ExecuteAsync(command.MobileNumber!);

        var sendResult = await _payamakSender.ExecuteAsync(new()
        {
            Message = $"{code}",
            Receptor = command.MobileNumber
        });

        if (sendResult.IsSuccess)
        {
            commandResult = CommandResult<LoginByOtpCommandResult?>.Success(new()
            {
                OtpSended = true,
                ExpirationTime = TimeSpan.FromSeconds(120)
            });
        }
        else
        {
            commandResult = CommandResult<LoginByOtpCommandResult?>.Failure("خطا در ارسال کد", new()
            {
                OtpSended = false
            });
        }

        return commandResult;
    }
}