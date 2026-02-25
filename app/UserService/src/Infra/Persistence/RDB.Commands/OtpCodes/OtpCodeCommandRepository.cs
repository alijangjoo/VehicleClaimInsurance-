using Insurance.UserService.AppCore.Domain.OtpCodes.Entities;

namespace Insurance.UserService.Infra.Persistence.RDB.Commands.OtpCodes
{
    public class OtpCodeCommandRepository  //IOtpCodeCommandRepository
    {
        public OtpCodeCommandRepository(InsuranceUserServiceDbContext dbContext) { }

        public async Task<OtpCode> GetByMobileNumberAndCode(string mobileNumber, string code)
        {
            return default!;
        }
    }
}