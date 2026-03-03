namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByCredential;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System.Threading.Tasks;

public class LoginByCredentialCommandHandler : CommandHandler<LoginByCredentialCommand, LoginByCredentialCommandResult?>
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly InsuranceUserServiceDbContext _dbContext;

    public LoginByCredentialCommandHandler(
        UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        ITokenService tokenService,
        ICacheServiceCaller cacheServiceCaller,
        InsuranceUserServiceDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _cacheServiceCaller = cacheServiceCaller;
        _dbContext = dbContext;
    }

    public override async Task<CommandResult<LoginByCredentialCommandResult?>> Handle(LoginByCredentialCommand command)
    {
        if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.Password))
            return CommandResult<LoginByCredentialCommandResult?>.Failure("نام کاربری یا رمز عبور وارد نشده است");

        var user = await _userManager.FindByNameAsync(command.UserName);
        if (user == null)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("کاربر یافت نشد");

        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
        if (!passwordCheck.Succeeded)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("نام کاربری یا رمز عبور اشتباه است");

        var memberships = await _dbContext.Memberships
            .Where(m => m.UserBusinessKey == user.UserBusinessKey && m.IsActive)
            .Join(_dbContext.MembershipRoleAssignments, m => m.BusinessKey, mra => mra.MembershipBusinessKey, (m, ra) => new MembershipDto
            {
                BusinessKey = m.BusinessKey.Value,
                OrganizationBusinessKey = m.OrganizationBusinessKey.Value,
                RoleBusinessKey = ra.RoleBusinessKey.Value
            }).ToListAsync();

        var membershipDtos = memberships.ToList();
        var membershipKeys = memberships.Select(c => c.BusinessKey).ToList();
        if (!membershipKeys.Any())
        {
            return await FailAsync("کاربر در هیچ سازمان فعالی عضویت ندارد");
        }

        var roleKeys = await _dbContext.MembershipRoleAssignments
            .Where(ra => membershipKeys.Contains(ra.MembershipBusinessKey.Value))
            .Select(ra => ra.RoleBusinessKey)
            .Distinct()
            .ToListAsync();

        var allPermissionCodes = await _dbContext.RolePermissions
            .Where(rp => roleKeys.Contains(rp.RoleBusinessKey))
            .Join(_dbContext.Permissions, rp => rp.PermissionBusinessKey, p => p.BusinessKey, (rp, p) => p.Code)
            .Distinct()
            .ToListAsync();

        // تولید توکن JWT
        var tokenResult = await _tokenService.GenerateTokenAsync(user, membershipDtos);

        // ذخیره Permissionها در کش
        await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
            Key: $"user--permissions--{user.UserBusinessKey.Value}",
            Value: string.Join(',', allPermissionCodes),
            AbsoluteExpirationMinutes: 60 // معادل زمان انقضا 60 دقیقه
        ));

        var result = new LoginByCredentialCommandResult
        {
            Token = tokenResult.AccessToken,
            TokenExpiration = tokenResult.Expiration,
            User = user,
            Memberships = membershipDtos,
            Permissions = allPermissionCodes
        };

        return CommandResult<LoginByCredentialCommandResult?>.Success(result);
    }
}