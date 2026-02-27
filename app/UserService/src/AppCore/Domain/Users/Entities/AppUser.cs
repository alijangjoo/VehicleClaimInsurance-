namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.Exceptions;
using OysterFx.AppCore.Domain.ValueObjects;

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

    public decimal MaxAllowedExpertiseAmount { get; private set; }
    public decimal MaxAllowedDailyCaseReferral { get; private set; }
    public decimal MaxAllowedOpenCases { get; private set; }

    public string? GPSDeviceId { get; private set; }
    public string? MobileNumber { get; private set; }

    public string? ProfileImageBase64 { get; private set; }

    public DateTime? UserExpirationDate { get; private set; }
    public DateTime? PasswordExpirationDate { get; private set; }

    private readonly List<UserMembership> _memberships = new();
    public IReadOnlyCollection<UserMembership> Memberships => _memberships.AsReadOnly();

    private AppUser() { }

    private AppUser(string mobileNumber, string userName)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
            throw new AggregateStateExceptions("Mobile number is required.", "");

        BusinessKey = BusinessKey.FromGuid(Guid.NewGuid());
        MobileNumber = mobileNumber;
        PhoneNumber = mobileNumber;
        UserName = userName;
        PhoneNumberConfirmed = true;
    }

    public static AppUser NewUser(string mobileNumber, string userName)
        => new(mobileNumber, userName);

    public void AddMembership(
        TenantId tenantId,
        BusinessKey organizationBusinessKey,
        DateTime startDate)
    {
        if (_memberships.Any(x =>
                x.TenantId == tenantId &&
                x.OrganizationBusinessKey == organizationBusinessKey &&
                x.IsActive))
            throw new AggregateStateExceptions("User already has active membership in this organization.", "");

        var membership = UserMembership.Create(
            this.BusinessKey,
            tenantId,
            organizationBusinessKey,
            startDate);

        _memberships.Add(membership);
    }

    public void DeactivateMembership(BusinessKey membershipBusinessKey)
    {
        var membership = _memberships
            .FirstOrDefault(x => x.BusinessKey == membershipBusinessKey);

        if (membership is null)
            throw new AggregateStateExceptions("Membership not found.", "");

        membership.Deactivate();
    }
}