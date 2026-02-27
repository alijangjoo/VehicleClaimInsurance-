namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;
using Insurance.UserService.Infra.Persistence.RDB.Commands.OtpCodes;
using Microsoft.Extensions.DependencyInjection;

public static class DIExtensions
{
    public static IServiceCollection AddUserServiceCommandPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IOtpCodeCommandRepository, OtpCodeCommandRepository>();
        return services;
    }
}