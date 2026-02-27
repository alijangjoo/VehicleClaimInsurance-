namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.Users.Dtos;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;


public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly InsuranceUserServiceDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        InsuranceUserServiceDbContext context,
        IAuthorizationService authorizationService,
        ILogger<UserContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<UserContext> BuildUserContextAsync(long userId, BusinessKey? membershipKey = null)
    {
        return default!;
        //var user = await _context.Users
        //    .Include(u => u.UserMemberships)
        //        .ThenInclude(um => um.Organization)
        //    .FirstOrDefaultAsync(u => u.Id == userId);

        //if (user == null)
        //    throw new InvalidOperationException($"User with ID {userId} not found");

        //var userContext = new UserContext
        //{
        //    UserId = user.Id,
        //    Username = user.UserName,
        //    FullName = user.FullName,
        //    Email = user.Email,
        //    UserBusinessKey = user.BusinessKey,
        //    IsAuthenticated = true,
        //    LastActivityAt = DateTime.UtcNow
        //};

        //// تمام عضویت‌های فعال کاربر
        //var memberships = user.UserMemberships
        //    .Where(um => um.IsActive)
        //    .ToList();

        //userContext.AvailableMemberships = memberships;

        //// عضویت فعلی
        //userContext.CurrentMembership = membershipKey != null
        //    ? memberships.FirstOrDefault(m => m.BusinessKey == membershipKey)
        //    : memberships.FirstOrDefault();

        //// سازمان‌های فعال کاربر
        //var organizations = memberships
        //    .Select(um => um.Organization)
        //    .Where(org => org != null)
        //    .Distinct()
        //    .ToList()!;

        //userContext.Organizations = organizations;

        //// سازمان فعلی
        //userContext.CurrentOrganization = userContext.CurrentMembership?.Organization;

        //// مجوزهای کاربر برای عضویت فعلی
        //if (userContext.CurrentMembership != null)
        //{
        //    var permissions = await _authorizationService
        //        .GetUserPermissionsAsync(user.Id, userContext.CurrentMembership.BusinessKey);
        //    userContext.Permissions = permissions.ToList();
        //}

        //// ذخیره در HttpContext.Items
        //_httpContextAccessor.HttpContext?.Items["UserContext"] = userContext;

        //return userContext;
    }

    public async Task<UserContext?> GetCurrentUserContextAsync()
    {
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue("UserContext", out var cached) == true)
            return cached as UserContext;

        var userId = GetCurrentUserId();
        if (!userId.HasValue) return null;

        var membershipKey = GetCurrentMembershipKey();
        return await BuildUserContextAsync(userId.Value, membershipKey);
    }

    public async Task<bool> SwitchMembershipAsync(BusinessKey newMembershipKey)
    {
        return default!;
        //var userId = GetCurrentUserId();
        //if (!userId.HasValue) return false;

        //// بررسی دسترسی کاربر به عضویت جدید
        //var hasAccess = await _context.UserMemberships
        //    .AnyAsync(um => um.UserBusinessKey == BusinessKey.FromInt((int)userId.Value)
        //                && um.BusinessKey == newMembershipKey);

        //if (!hasAccess) return false;

        //// بروزرسانی Claim در Identity
        //var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        //if (identity != null)
        //{
        //    var oldClaim = identity.FindFirst("membershipKey");
        //    if (oldClaim != null) identity.RemoveClaim(oldClaim);
        //    identity.AddClaim(new Claim("membershipKey", newMembershipKey.Value));
        //}

        //// بازسازی UserContext با عضویت جدید
        //await BuildUserContextAsync(userId.Value, newMembershipKey);
        //return true;
    }

    public async Task UpdateLastActivityAsync()
    {
        var context = await GetCurrentUserContextAsync();
        if (context != null)
            context.LastActivityAt = DateTime.UtcNow;
    }

    public Task ClearUserContextAsync()
    {
        _httpContextAccessor.HttpContext?.Items.Remove("UserContext");
        return Task.CompletedTask;
    }

    private long? GetCurrentUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return claim != null && long.TryParse(claim, out var id) ? id : null;
    }

    private BusinessKey? GetCurrentMembershipKey()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("membershipKey")?.Value;
        return claim != null ? BusinessKey.FromGuid(Guid.Parse(claim)) : null;
    }

    public Task<bool> SwitchPersonaAsync(BusinessKey newPersonaKey)
    {
        throw new NotImplementedException();
    }
}