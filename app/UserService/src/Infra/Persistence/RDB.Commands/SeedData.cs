namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System;
using System.Linq;

public static class SeedData
{
    private static readonly Guid SystemOrgBusinessKey = Guid.Parse("27632bf0-4ab2-4aa6-a606-b1e5d0135258");

    private static readonly Guid User1BusinessKey = Guid.Parse("ef58349f-d52c-4c5c-9966-0ec7c14f3c51");
    private static readonly Guid User2BusinessKey = Guid.Parse("0bb85299-367b-4d68-8db1-5e66003a5522");
    private static readonly Guid User3BusinessKey = Guid.Parse("9e500e63-7399-4afa-87e2-cc7f5e1f9065");

    private static readonly Guid Permission1BusinessKey = Guid.Parse("3dd4ec06-a23c-41fd-acc2-62ffe2bc7ea6");
    private static readonly Guid Permission2BusinessKey = Guid.Parse("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7");
    private static readonly Guid Permission3BusinessKey = Guid.Parse("82514bf2-ebb6-4e75-84a5-9c4d8831121d");
    private static readonly Guid Permission4BusinessKey = Guid.Parse("874b7fe9-b8b6-41f5-b00c-b611ede1634e");

    private static readonly Guid Role1BusinessKey = Guid.Parse("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96");
    private static readonly Guid Role2BusinessKey = Guid.Parse("17772f1c-d812-4a18-b30e-a30068d8ca57");
    private static readonly Guid Role3BusinessKey = Guid.Parse("3ee32a19-e5f1-468d-baf1-321761ed8b60");

    private static readonly DateTime FixedDate = new DateTime(2026, 2, 23, 22, 5, 5, DateTimeKind.Utc);
    private static readonly DateTime FixedUserExpirationDate = new DateTime(2027, 2, 23, 22, 5, 5, DateTimeKind.Utc);
    private static readonly DateTime FixedPasswordExpirationDate = new DateTime(2026, 5, 23, 22, 5, 5, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedOrganization(modelBuilder);
        SeedPermissions(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);
        SeedUserMemberships(modelBuilder);
        SeedRolePermissions(modelBuilder);
    }

    private static void SeedOrganization(ModelBuilder modelBuilder)
    {
        var tenantId = TenantId.FromString("TENANT_DEFAULT");

        modelBuilder.Entity<Organization>().HasData(
            new
            {
                Id = 1L,
                Name = "شرکت نمونه داده‌پرداز",
                IsActive = true,
                BusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey),
                TenantId = tenantId,  // Pass the TenantId object directly, not tenantId.Value
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        );
    }

    private static void SeedPermissions(ModelBuilder modelBuilder)
    {
        var permissions = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Permission1BusinessKey), Code = "USER_CREATE", Scope = RoleScope.System, Description = "ایجاد کاربر جدید", IsActive = true },
            new { Id = 2L, BusinessKey = BusinessKey.FromGuid(Permission2BusinessKey), Code = "Losts.AssignHullThirdPartyInsurerCode", Scope = RoleScope.System, Description = "صدور کد بیمه گران طرف قرارداد بدنه", IsActive = true },
            new { Id = 3L, BusinessKey = BusinessKey.FromGuid(Permission3BusinessKey), Code = "Losts.AssignThirdPartyInsurerCode", Scope = RoleScope.System, Description = "صدور کد بیمه گران طرف قرارداد ثالث مالی", IsActive = true },
            new { Id = 4L, BusinessKey = BusinessKey.FromGuid(Permission4BusinessKey), Code = "ROLE_ASSIGN", Scope = RoleScope.Organization, Description = "اختصاص نقش به کاربران", IsActive = true }
        };

        modelBuilder.Entity<Permission>().HasData(
            permissions.Select(p => new
            {
                p.Id,
                p.BusinessKey,
                p.Code,
                p.Scope,
                p.Description,
                p.IsActive,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }).ToArray()
        );
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var tenantId = TenantId.FromString("TENANT_DEFAULT");

        var roles = new[]
        {
        new
        {
            Id = 1L,
            BusinessKey = BusinessKey.FromGuid(Role1BusinessKey),  // ✅ Fixed: pass the BusinessKey object, not .Value
            Name = "مدیر سیستم",
            Scope = RoleScope.System,
            NormalizedName = "مدیر سیستم",
            TenantId = tenantId,  // Pass the TenantId object directly
            ConcurrencyStamp = (string?)null
        },
        new
        {
            Id = 2L,
            BusinessKey = BusinessKey.FromGuid(Role2BusinessKey),  // ✅ Fixed
            Name = "مدیر سازمان",
            Scope = RoleScope.Organization,
            NormalizedName = "مدیر سازمان",
            TenantId = tenantId,  // Pass the TenantId object directly
            ConcurrencyStamp = (string?)null
        },
        new
        {
            Id = 3L,
            BusinessKey = BusinessKey.FromGuid(Role3BusinessKey),  // ✅ Fixed
            Name = "کاربر عادی",
            Scope = RoleScope.Application,
            NormalizedName = "کاربر عادی",
            TenantId = tenantId,  // Pass the TenantId object directly
            ConcurrencyStamp = (string?)null
        }
    };

        modelBuilder.Entity<AppRole>().HasData(roles);
    }
    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        const string passwordHash = "AQAAAAIAAYagAAAAEK3B8o7QzL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5QnL5Q==";

        var users = new[]
        {
        new
        {
            Id = 1L,
            BusinessKey = BusinessKey.FromGuid(User1BusinessKey),
            Email = "admin@example.com",
            UserName = "admin",
            Code = "AD001",
            FullName = "مدیر ارشد",
            EmailConfirmed = true,
            PasswordHash = passwordHash,
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            NormalizedUserName = "ADMIN",
            // Required Identity properties
            AccessFailedCount = 0,
            LockoutEnabled = false,
            LockoutEnd = (DateTimeOffset?)null,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            PhoneNumber = (string?)null,
            PhoneNumberConfirmed = false,
            // Custom required decimal properties - all set to 0 (unlimited) for admin
            MaxAllowedExpertiseAmount = 0m,
            MaxAllowedDailyCaseReferral = 0m,
            MaxAllowedOpenCases = 0m,
            // Optional properties
            City = "null",
            Province = "null",
            GPSDeviceId = "null",
            MobileNumber = "null",
            ProfileImageBase64 = "null",
            UserExpirationDate = "null",
            PasswordExpirationDate = "null"
        },
        new
        {
            Id = 2L,
            BusinessKey = BusinessKey.FromGuid(User2BusinessKey),
            Email = "expert@example.com",
            UserName = "expert",
            Code = "EX001",
            FullName = "کارشناس فنی",
            EmailConfirmed = true,
            PasswordHash = passwordHash,
            NormalizedEmail = "EXPERT@EXAMPLE.COM",
            NormalizedUserName = "EXPERT",
            // Required Identity properties
            AccessFailedCount = 0,
            LockoutEnabled = false,
            LockoutEnd = (DateTimeOffset?)null,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            PhoneNumber = (string?)null,
            PhoneNumberConfirmed = false,
            // Custom required decimal properties - set reasonable limits for expert
            MaxAllowedExpertiseAmount = 1000000m,  // 1,000,000 units
            MaxAllowedDailyCaseReferral = 50m,      // 50 cases per day
            MaxAllowedOpenCases = 20m,              // 20 open cases at once
            // Optional properties
            City = "null",
            Province = "null",
            GPSDeviceId = "null",
            MobileNumber = "null",
            ProfileImageBase64 = "null",
            UserExpirationDate = "null",
            PasswordExpirationDate = "null"
        },
        new
        {
            Id = 3L,
            BusinessKey = BusinessKey.FromGuid(User3BusinessKey),
            Email = "support@example.com",
            UserName = "support",
            Code = "SU001",
            FullName = "پشتیبان",
            EmailConfirmed = true,
            PasswordHash = passwordHash,
            NormalizedEmail = "SUPPORT@EXAMPLE.COM",
            NormalizedUserName = "SUPPORT",
            // Required Identity properties
            AccessFailedCount = 0,
            LockoutEnabled = false,
            LockoutEnd = (DateTimeOffset?)null,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            PhoneNumber = (string?)null,
            PhoneNumberConfirmed = false,
            // Custom required decimal properties - set lower limits for support
            MaxAllowedExpertiseAmount = 500000m,    // 500,000 units
            MaxAllowedDailyCaseReferral = 30m,      // 30 cases per day
            MaxAllowedOpenCases = 10m,               // 10 open cases at once
            // Optional properties
            City = "null",
            Province = "null",
            GPSDeviceId = "null",
            MobileNumber = "null",
            ProfileImageBase64 = "null",
            UserExpirationDate = "null",
            PasswordExpirationDate = "null"
        }
    };

        modelBuilder.Entity<AppUser>().HasData(users);
    }

    private static void SeedUserMemberships(ModelBuilder modelBuilder)
    {
        var userMemberships = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), UserBusinessKey = BusinessKey.FromGuid(User1BusinessKey), OrganizationBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), AssignmentType = AssignmentType.Direct, StartDate = FixedDate, IsActive = true },
            new { Id = 2L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), UserBusinessKey = BusinessKey.FromGuid(User2BusinessKey), OrganizationBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey), RoleBusinessKey = BusinessKey.FromGuid(Role2BusinessKey), AssignmentType = AssignmentType.Direct, StartDate = FixedDate, IsActive = true },
            new { Id = 3L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), UserBusinessKey = BusinessKey.FromGuid(User3BusinessKey), OrganizationBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey), RoleBusinessKey = BusinessKey.FromGuid(Role3BusinessKey), AssignmentType = AssignmentType.Direct, StartDate = FixedDate, IsActive = true }
        };

        modelBuilder.Entity<UserMembership>().HasData(userMemberships);
    }

    private static void SeedRolePermissions(ModelBuilder modelBuilder)
    {
        var rolePermissions = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission1BusinessKey) },
            new { Id = 2L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission2BusinessKey) },
            new { Id = 3L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role2BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission3BusinessKey) },
            new { Id = 4L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role3BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission4BusinessKey) }
        };

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }
}