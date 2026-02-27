namespace Insurance.UserService.Infra.Persistence.RDB.Commands.OtpCodes;

using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;

public class OtpCodeCommandRepository : IOtpCodeCommandRepository
{
    public OtpCodeCommandRepository(InsuranceUserServiceDbContext dbContext) { }

    public async Task<OtpCode> GetByMobileNumberAndCode(string mobileNumber, string code)
    {
        return default!;
    }
}