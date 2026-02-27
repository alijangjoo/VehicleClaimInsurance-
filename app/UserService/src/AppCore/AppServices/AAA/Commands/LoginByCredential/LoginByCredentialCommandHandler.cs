namespace Insurance.UserService.AppCore.AppServices.AAA.Commands.LoginByCredential;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
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
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly InsuranceUserServiceDbContext _dbContext;

    public LoginByCredentialCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
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

        // پیدا کردن کاربر
        var user = await _userManager.FindByNameAsync(command.UserName);
        if (user == null)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("کاربر یافت نشد");

        // بررسی پسورد
        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
        if (!passwordCheck.Succeeded)
            return CommandResult<LoginByCredentialCommandResult?>.Failure("نام کاربری یا رمز عبور اشتباه است");

        // گرفتن عضویت‌ها (Memberships) کاربر
        var memberships = await _dbContext.UserMemberships
            .Include(m => m.RoleAssignments) // RoleAssignments مربوط به هر Membership
            .Where(m => m.UserBusinessKey == user.BusinessKey && m.IsActive)
            .ToListAsync();

        // استخراج RoleBusinessKeys و سپس Permission Codes
        var allPermissionCodes = new List<string>();
        var membershipDtos = new List<UserMembership>();

        foreach (var membership in memberships)
        {
            membershipDtos.Add(membership); // برای خروجی

            // گرفتن RoleBusinessKey ها
            var roleKeys = membership.RoleAssignments.Select(ra => ra.RoleBusinessKey).ToList();

            if (!roleKeys.Any())
                continue;

            // گرفتن PermissionCodeها از RolePermission
            var permissionCodes = await _dbContext.RolePermissions
                .Where(rp => roleKeys.Contains(rp.RoleBusinessKey))
                .Join(_dbContext.Permissions,
                      rp => rp.PermissionBusinessKey,
                      p => p.BusinessKey,
                      (rp, p) => p.Code)
                .ToListAsync();

            allPermissionCodes.AddRange(permissionCodes);
        }

        allPermissionCodes = allPermissionCodes.Distinct().ToList();

        // تولید توکن JWT
        var tokenResult = await _tokenService.GenerateTokenAsync(user, membershipDtos);

        // ذخیره Permissionها در کش
        await _cacheServiceCaller.SetAsync(new SetToCacheRequest(
            Key: $"user--permissions--{user.BusinessKey.Value}",
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