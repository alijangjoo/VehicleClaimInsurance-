using Insurance.UserService.AppCore.Domain.Users.Dtos;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;

namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<UserContextService> _logger;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        ILogger<UserContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<UserContext> BuildUserContextAsync(long userId, BusinessKey? personaKey = null)
    {
        try
        {
            await _semaphore.WaitAsync();

            var user = await _context.Users
                .Include(u => u.UserPersonas)
                    .ThenInclude(up => up.Persona)
                .Include(u => u.UserOrganizations)
                    .ThenInclude(uo => uo.Organization)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            var userContext = new UserContext
            {
                UserId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                UserBusinessKey = user.BusinessKey,
                IsAuthenticated = true,
                LastActivityAt = DateTime.UtcNow
            };

            // دریافت Personaهای فعال کاربر
            var userPersonas = user.UserPersonas
                .Where(up => up.Persona != null)
                .Select(up => up.Persona!)
                .ToList();

            userContext.AvailablePersonas = userPersonas;

            // تعیین Persona فعلی
            if (personaKey != null && userPersonas.Any(p => p.BusinessKey == personaKey))
            {
                userContext.CurrentPersonaKey = personaKey;
                userContext.CurrentPersona = userPersonas.First(p => p.BusinessKey == personaKey);
            }
            else if (userPersonas.Any())
            {
                // انتخاب اولین Persona به عنوان پیش‌فرض
                userContext.CurrentPersonaKey = userPersonas.First().BusinessKey;
                userContext.CurrentPersona = userPersonas.First();
            }

            // دریافت سازمان‌های کاربر
            var organizations = user.UserOrganizations
                .Where(uo => uo.IsActive && uo.Organization != null)
                .Select(uo => uo.Organization!)
                .ToList();

            userContext.Organizations = organizations;

            // دریافت سازمان فعلی (بر اساس Persona انتخاب شده)
            if (userContext.CurrentPersona != null)
            {
                userContext.CurrentOrganization = organizations
                    .FirstOrDefault(o => o.BusinessKey == userContext.CurrentPersona.OrganizationBusinessKey);
            }

            // دریافت مجوزهای کاربر برای Persona فعلی
            if (userContext.CurrentPersonaKey != null)
            {
                var permissions = await _authorizationService
                    .GetUserPermissionsAsync(user.Id, userContext.CurrentPersonaKey);
                userContext.Permissions = permissions.ToList();
            }

            // ذخیره در HttpContext Items برای استفاده در طول request
            _httpContextAccessor.HttpContext?.Items.TryAdd("UserContext", userContext);

            return userContext;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<UserContext?> GetCurrentUserContextAsync()
    {
        // ابتدا از HttpContext.Items بررسی می‌کنیم
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue("UserContext", out var cachedContext) == true)
        {
            return cachedContext as UserContext;
        }

        // اگر در HttpContext نبود، از Claims توکن می‌سازیم
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return null;

        var personaKey = GetCurrentPersonaKey();

        return await BuildUserContextAsync(userId.Value, personaKey);
    }

    public async Task<bool> SwitchPersonaAsync(BusinessKey newPersonaKey)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return false;

        // بررسی دسترسی به Persona جدید
        var hasAccess = await _context.UserPersonas
            .AnyAsync(up => up.UserBusinessKey == BusinessKey.FromInt((int)userId.Value) &&
                           up.PersonaBusinessKey == newPersonaKey);

        if (!hasAccess)
            return false;

        // به‌روزرسانی HttpContext
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true)
        {
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var oldPersonaClaim = identity?.FindFirst("personaKey");
            if (oldPersonaClaim != null)
                identity?.RemoveClaim(oldPersonaClaim);

            identity?.AddClaim(new Claim("personaKey", newPersonaKey.Value));
        }

        // بازسازی UserContext
        await BuildUserContextAsync(userId.Value, newPersonaKey);

        return true;
    }

    public async Task UpdateLastActivityAsync()
    {
        var context = await GetCurrentUserContextAsync();
        if (context != null)
        {
            context.LastActivityAt = DateTime.UtcNow;
        }
    }

    public Task ClearUserContextAsync()
    {
        _httpContextAccessor.HttpContext?.Items.Remove("UserContext");
        return Task.CompletedTask;
    }

    private long? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return userIdClaim != null ? long.Parse(userIdClaim) : null;
    }

    private BusinessKey? GetCurrentPersonaKey()
    {
        var personaKeyClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("personaKey")?.Value;
        return personaKeyClaim != null ? BusinessKey.FromString(personaKeyClaim) : null;
    }
}