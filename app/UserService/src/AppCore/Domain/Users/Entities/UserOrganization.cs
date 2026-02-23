namespace Insurance.UserService.AppCore.Domain.Users.Entities;

using Insurance.UserService.AppCore.Domain.Users.Enums;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public class UserOrganization : Entity<long>
{
    public BusinessKey UserBusinessKey { get; private set; }
    public BusinessKey OrganizationBusinessKey { get; private set; }
    
    /// <summary>
    /// نوع انتصاب: مستقیم یا از طریق Persona
    /// </summary>
    public AssignmentType AssignmentType { get; private set; }

    /// <summary>
    /// اگر انتصاب از طریق Persona باشد، این فیلد مقداردهی می‌شود
    /// </summary>
    public BusinessKey? PersonaBusinessKey { get; private set; }

    /// <summary>
    /// تاریخ شروع انتصاب
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// تاریخ پایان انتصاب (اختیاری)
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// وضعیت فعال/غیرفعال
    /// </summary>
    public bool IsActive { get; private set; }

    private UserOrganization() { }

    private UserOrganization(
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey,
        AssignmentType assignmentType,
        BusinessKey? personaBusinessKey = null)
    {
        UserBusinessKey = userBusinessKey;
        OrganizationBusinessKey = organizationBusinessKey;
        AssignmentType = assignmentType;
        PersonaBusinessKey = personaBusinessKey;
        StartDate = DateTime.UtcNow;
        IsActive = true;
    }

    public static UserOrganization AssignDirectly(
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey)
    {
        return new UserOrganization(
            userBusinessKey,
            organizationBusinessKey,
            AssignmentType.Direct);
    }

    public static UserOrganization AssignViaPersona(
        BusinessKey userBusinessKey,
        BusinessKey organizationBusinessKey,
        BusinessKey personaBusinessKey)
    {
        return new UserOrganization(
            userBusinessKey,
            organizationBusinessKey,
            AssignmentType.ViaPersona,
            personaBusinessKey);
    }

    public void Deactivate() => IsActive = false;

    public void ExtendAssignment(DateTime newEndDate)
    {
        if (newEndDate <= StartDate)
            throw new InvalidOperationException("تاریخ پایان باید بعد از تاریخ شروع باشد");

        EndDate = newEndDate;
    }
}
