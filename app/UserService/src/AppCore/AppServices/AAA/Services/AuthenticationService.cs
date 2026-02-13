namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly InsuranceUserServiceDbContext _context;

    public AuthenticationService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        InsuranceUserServiceDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return AuthResult.Failed("کاربر یافت نشد");

        // بررسی انقضای کاربر
        if (user.UserExpirationDate.HasValue && user.UserExpirationDate < DateTime.UtcNow)
            return AuthResult.Failed("حساب کاربری منقضی شده است");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        if (result.Succeeded)
        {
            // دریافت Personaهای فعال کاربر
            var personas = await _context.UserPersonas
                .Where(up => up.UserBusinessKey == user.BusinessKey)
                .Select(up => up.Persona)
                .ToListAsync();

            var token = await _tokenService.GenerateTokenAsync(user, personas);

            return AuthResult.Success(token, user, personas);
        }

        if (result.IsLockedOut)
            return AuthResult.Failed("حساب کاربری قفل شده است");

        return AuthResult.Failed("نام کاربری یا رمز عبور اشتباه است");
    }

    public async Task<AuthResult> LoginWithPersonaAsync(string username, string password, BusinessKey personaKey)
    {
        var result = await LoginAsync(username, password);
        if (!result.IsSuccess)
            return result;

        // بررسی دسترسی به Persona انتخاب شده
        var hasAccess = await _context.UserPersonas
            .AnyAsync(up => up.UserBusinessKey == result.User.BusinessKey &&
                           up.PersonaBusinessKey == personaKey);

        if (!hasAccess)
            return AuthResult.Failed("دسترسی به این persona وجود ندارد");

        // ایجاد توکن با Persona خاص
        var token = await _tokenService.GenerateTokenWithPersonaAsync(result.User, personaKey);

        return AuthResult.Success(token, result.User, new[] { await _context.Personas.FindAsync(personaKey) });
    }
}