using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.ValueObjects;

namespace Insurance.UserService.AppCore.Domain.Users.Entities;

/// <summary>
/// کاربر -Aggregate Root
/// </summary>
public sealed class AppUser : IdentityUser<long>
{
    public BusinessKey BusinessKey { get; private set; }
    public string? Code { get; private set; }
    public string? FullName { get; private set; }
    public string? City { get; private set; }
    public string? Province { get; private set; }

    /// <summary>
    /// سقف مجاز کارشناسی
    /// </summary>
    public decimal MaxAllowedExpertiseAmount { get; private set; } // 0 = unlimited

    /// <summary>
    /// سقف مجاز ارجاع پرونده در روز
    /// </summary>
    public decimal MaxAllowedDailyCaseReferral { get; private set; } // 0 = unlimited

    /// <summary>
    /// سقف مجاز پرونده‌های باز
    /// </summary>
    public decimal MaxAllowedOpenCases { get; private set; } // 0 = unlimited

    /// <summary>
    /// شناسه دستگاه GPS
    /// </summary>
    public string? GPSDeviceId { get; private set; }
    public string? MobileNumber { get; private set; }

    /// <summary>
    /// تصویر پروفایل به صورت Base64
    /// </summary>
    public string? ProfileImageBase64 { get; private set; }

    /// <summary>
    /// تاریخ انقضای کاربر
    /// </summary>
    public DateTime? UserExpirationDate { get; private set; }

    /// <summary>
    /// تاریخ انقضای رمز عبور
    /// </summary>
    public DateTime? PasswordExpirationDate { get; private set; }

    private readonly List<UserOrganization> _userOrganizations = new();
    public IReadOnlyCollection<UserOrganization> UserOrganizations => _userOrganizations.AsReadOnly();

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<UserPersona> _userPersonas = new();
    public IReadOnlyCollection<UserPersona> UserPersonas => _userPersonas.AsReadOnly();

    private AppUser() { }

    private AppUser(string email, string? username, string? code)
    {
        Email = email;
        UserName = username;
        Code = code;
    }

    private AppUser(string mobileNumber, string? username)
    {
        MobileNumber = mobileNumber;
        PhoneNumber = mobileNumber;
        UserName = username;
        PhoneNumberConfirmed = true;
    }

    public static AppUser NewUser(string mobileNumber, string userName) => new(mobileNumber, userName);

    public void AddRole(AppRole role, OrganizationPersona? persona = null)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));

        ValidateRoleAssignment(role, persona);

        var userRole = UserRole.AssignUserRole(BusinessKey, role.BusinessKey, persona.BusinessKey);
        _userRoles.Add(userRole);
    }

    public void AddPersona(OrganizationPersona persona)
    {
        if (persona == null) throw new ArgumentNullException(nameof(persona));

        var userPersona = UserPersona.AssignUserPersona(BusinessKey, persona.BusinessKey);
        _userPersonas.Add(userPersona);
    }

    private void ValidateRoleAssignment(AppRole role, OrganizationPersona? persona)
    {
        switch (role.Scope)
        {
            case RoleScope.System:
                if (persona != null)
                    throw new InvalidOperationException("System roles cannot be assigned to personas");
                break;
            case RoleScope.Organization:
            case RoleScope.Application:
                if (persona == null)
                    throw new InvalidOperationException($"Role scope '{role.Scope}' requires a persona");
                break;
        }
    }
}