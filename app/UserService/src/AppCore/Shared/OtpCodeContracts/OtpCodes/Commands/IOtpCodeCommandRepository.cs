namespace Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using OysterFx.AppCore.Shared.Commands;
using System.Threading.Tasks;

public interface IOtpCodeCommandRepository : ICommandRepository<OtpCode, long>
{
    Task<OtpCode> GetByMobileNumberAndCode(string mobileNumber, string code);
}