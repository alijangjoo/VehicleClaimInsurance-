using Insurance.UserService.AppCore.Domain.Users.Dtos;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.AspNetCore.Http;
using OysterFx.AppCore.Domain.ValueObjects;

namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContextService _userContextService;
    private readonly IAuthorizationService _authorizationService;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUserContextService userContextService,
        IAuthorizationService authorizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userContextService = userContextService;
        _authorizationService = authorizationService;
    }

    public long? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            return userIdClaim != null ? long.Parse(userIdClaim) : null;
        }
    }

    public BusinessKey? UserBusinessKey
    {
        get
        {
            var businessKeyClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("businessKey")?.Value;
            return businessKeyClaim != null ? BusinessKey.FromString(businessKeyClaim) : null;
        }
    }

    public BusinessKey? CurrentPersonaKey
    {
        get
        {
            var personaKeyClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("personaKey")?.Value;
            return personaKeyClaim != null ? BusinessKey.FromString(personaKeyClaim) : null;
        }
    }

    public bool IsAuthenticated => UserId.HasValue;

    public async Task<UserContext> GetCurrentUserContextAsync()
    {
        return await _userContextService.GetCurrentUserContextAsync() ?? new UserContext();
    }

    public async Task<bool> HasPermissionAsync(string permissionCode)
    {
        if (!UserId.HasValue || !CurrentPersonaKey.HasValue)
            return false;

        return await _authorizationService.HasPermissionAsync(
            UserId.Value,
            permissionCode,
            CurrentPersonaKey.Value);
    }

    public async Task<List<string>> GetCurrentUserPermissionsAsync()
    {
        var context = await GetCurrentUserContextAsync();
        return context.Permissions;
    }
}
