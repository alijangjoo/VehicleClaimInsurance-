namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System;
using System.Linq;

public static class SeedData
{
    // Define static, deterministic GUIDs for all seed data
    private static readonly Guid SystemOrgBusinessKey = Guid.Parse("27632bf0-4ab2-4aa6-a606-b1e5d0135258");

    // User GUIDs
    private static readonly Guid User1BusinessKey = Guid.Parse("ef58349f-d52c-4c5c-9966-0ec7c14f3c51");
    private static readonly Guid User2BusinessKey = Guid.Parse("0bb85299-367b-4d68-8db1-5e66003a5522");
    private static readonly Guid User3BusinessKey = Guid.Parse("9e500e63-7399-4afa-87e2-cc7f5e1f9065");

    // Persona GUIDs
    private static readonly Guid Persona1BusinessKey = Guid.Parse("eb1598b6-8a1c-445f-aab2-208aad4941b6");
    private static readonly Guid Persona2BusinessKey = Guid.Parse("c7b76748-07c3-4030-98fc-c1740002c00e");
    private static readonly Guid Persona3BusinessKey = Guid.Parse("99ff2684-86d1-45d4-9df8-e1a982083bcc");

    // Permission GUIDs
    private static readonly Guid Permission1BusinessKey = Guid.Parse("3dd4ec06-a23c-41fd-acc2-62ffe2bc7ea6");
    private static readonly Guid Permission2BusinessKey = Guid.Parse("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7");
    private static readonly Guid Permission3BusinessKey = Guid.Parse("82514bf2-ebb6-4e75-84a5-9c4d8831121d");
    private static readonly Guid Permission4BusinessKey = Guid.Parse("874b7fe9-b8b6-41f5-b00c-b611ede1634e");

    // Role GUIDs
    private static readonly Guid Role1BusinessKey = Guid.Parse("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96");
    private static readonly Guid Role2BusinessKey = Guid.Parse("17772f1c-d812-4a18-b30e-a30068d8ca57");
    private static readonly Guid Role3BusinessKey = Guid.Parse("3ee32a19-e5f1-468d-baf1-321761ed8b60");

    // تاریخ‌های ثابت از migration
    private static readonly DateTime FixedDate = new DateTime(2026, 2, 23, 22, 5, 5, DateTimeKind.Utc);
    private static readonly DateTime FixedUserExpirationDate = new DateTime(2027, 2, 23, 22, 5, 5, DateTimeKind.Utc);
    private static readonly DateTime FixedPasswordExpirationDate = new DateTime(2026, 5, 23, 22, 5, 5, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        // 1. Independent entities first
        SeedOrganization(modelBuilder);
        SeedPermissions(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);

        // 2. Entities that depend on the above
        SeedPersonas(modelBuilder);
        SeedRolePermissions(modelBuilder);

        // 3. Junction tables last
        SeedUserOrganizations(modelBuilder);
        SeedUserPersonas(modelBuilder);
        SeedUserRoles(modelBuilder);
    }

    private static void SeedOrganization(ModelBuilder modelBuilder)
    {
        var orgBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey);

        modelBuilder.Entity<Organization>().HasData(
            new
            {
                Id = 1L,
                Name = "شرکت نمونه داده‌پرداز",
                IsActive = true,
                BusinessKey = orgBusinessKey,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        );
    }

    private static void SeedPermissions(ModelBuilder modelBuilder)
    {
        var permission1BusinessKey = BusinessKey.FromGuid(Permission1BusinessKey);
        var permission2BusinessKey = BusinessKey.FromGuid(Permission2BusinessKey);
        var permission3BusinessKey = BusinessKey.FromGuid(Permission3BusinessKey);
        var permission4BusinessKey = BusinessKey.FromGuid(Permission4BusinessKey);

        var permissions = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = permission1BusinessKey,
                Code = "USER_CREATE",
                Scope = RoleScope.System,
                Description = "ایجاد کاربر جدید",
                IsActive = true
            },
            new
            {
                Id = 2L,
                BusinessKey = permission2BusinessKey,
                Code = "Losts.AssignHullThirdPartyInsurerCode",
                Scope = RoleScope.System,
                Description = "صدور کد بیمه گران طرف قرارداد بدنه",
                IsActive = true
            },
            new
            {
                Id = 3L,
                BusinessKey = permission3BusinessKey,
                Code = "Losts.AssignThirdPartyInsurerCode",
                Scope = RoleScope.System,
                Description = "صدور کد بیمه گران طرف قرارداد ثالث مالی",
                IsActive = true
            },
            new
            {
                Id = 4L,
                BusinessKey = permission4BusinessKey,
                Code = "ROLE_ASSIGN",
                Scope = RoleScope.Organization,
                Description = "اختصاص نقش به کاربران",
                IsActive = true
            }
        };

        modelBuilder.Entity<Permission>().HasData(
            permissions.Select(p => new
            {
                Id = p.Id,
                BusinessKey = p.BusinessKey,
                Code = p.Code,
                Scope = p.Scope,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }).ToArray()
        );
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var role1BusinessKey = BusinessKey.FromGuid(Role1BusinessKey);
        var role2BusinessKey = BusinessKey.FromGuid(Role2BusinessKey);
        var role3BusinessKey = BusinessKey.FromGuid(Role3BusinessKey);

        var roles = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = role1BusinessKey,
                Name = "مدیر سیستم",
                Scope = RoleScope.System,
                NormalizedName = "مدیر سیستم"
            },
            new
            {
                Id = 2L,
                BusinessKey = role2BusinessKey,
                Name = "مدیر سازمان",
                Scope = RoleScope.Organization,
                NormalizedName = "مدیر سازمان"
            },
            new
            {
                Id = 3L,
                BusinessKey = role3BusinessKey,
                Name = "کاربر عادی",
                Scope = RoleScope.Application,
                NormalizedName = "کاربر عادی"
            }
        };

        modelBuilder.Entity<AppRole>().HasData(
            roles.Select(r => new
            {
                Id = r.Id,
                BusinessKey = r.BusinessKey,
                Name = r.Name,
                Scope = r.Scope,
                NormalizedName = r.NormalizedName,
                ConcurrencyStamp = (string?)null
            }).ToArray()
        );
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        var user1BusinessKey = BusinessKey.FromGuid(User1BusinessKey);
        var user2BusinessKey = BusinessKey.FromGuid(User2BusinessKey);
        var user3BusinessKey = BusinessKey.FromGuid(User3BusinessKey);

        var users = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = user1BusinessKey,
                Email = "admin@example.com",
                UserName = "admin",
                Code = "AD001",
                FullName = "مدیر ارشد",
                City = "تهران",
                Province = "تهران",
                MaxAllowedExpertiseAmount = 10000000m,
                MaxAllowedDailyCaseReferral = 50m,
                MaxAllowedOpenCases = 100m,
                GPSDeviceId = "GPS001",
                MobileNumber = "09121111111",
                UserExpirationDate = FixedUserExpirationDate,
                PasswordExpirationDate = FixedPasswordExpirationDate,
                ProfileImageBase64 = (string?)null,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAEI...",
                SecurityStamp = "30d09939-a2ab-4840-afe8-5eb302182d21",
                ConcurrencyStamp = "fa2c7f2e-e506-4154-bcfb-d2f11e4bdeeb",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                NormalizedUserName = "ADMIN",
                PhoneNumber = (string?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = user2BusinessKey,
                Email = "expert@example.com",
                UserName = "expert",
                Code = "EX001",
                FullName = "کارشناس فنی",
                City = "تهران",
                Province = "تهران",
                MaxAllowedExpertiseAmount = 5000000m,
                MaxAllowedDailyCaseReferral = 30m,
                MaxAllowedOpenCases = 50m,
                GPSDeviceId = "GPS002",
                MobileNumber = "09122222222",
                UserExpirationDate = FixedUserExpirationDate,
                PasswordExpirationDate = FixedPasswordExpirationDate,
                ProfileImageBase64 = (string?)null,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAEI...",
                SecurityStamp = "60d0bb73-efe5-4e30-8dcf-2827606b892d",
                ConcurrencyStamp = "a217f7eb-5508-4e02-8566-95a4a1bdab7c",
                NormalizedEmail = "EXPERT@EXAMPLE.COM",
                NormalizedUserName = "EXPERT",
                PhoneNumber = (string?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = user3BusinessKey,
                Email = "support@example.com",
                UserName = "support",
                Code = "SU001",
                FullName = "پشتیبان",
                City = "تهران",
                Province = "تهران",
                MaxAllowedExpertiseAmount = 0m,
                MaxAllowedDailyCaseReferral = 20m,
                MaxAllowedOpenCases = 30m,
                GPSDeviceId = "GPS003",
                MobileNumber = "09123333333",
                UserExpirationDate = FixedUserExpirationDate,
                PasswordExpirationDate = FixedPasswordExpirationDate,
                ProfileImageBase64 = (string?)null,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAIAAYagAAAAEI...",
                SecurityStamp = "0653d8d2-7653-45d7-8215-c07526e07404",
                ConcurrencyStamp = "f1283964-4378-45f5-8815-57f1ce714caa",
                NormalizedEmail = "SUPPORT@EXAMPLE.COM",
                NormalizedUserName = "SUPPORT",
                PhoneNumber = (string?)null
            }
        };

        modelBuilder.Entity<AppUser>().HasData(users);
    }

    private static void SeedPersonas(ModelBuilder modelBuilder)
    {
        var orgBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey);
        var user1BusinessKey = BusinessKey.FromGuid(User1BusinessKey);
        var user2BusinessKey = BusinessKey.FromGuid(User2BusinessKey);
        var user3BusinessKey = BusinessKey.FromGuid(User3BusinessKey);
        var persona1BusinessKey = BusinessKey.FromGuid(Persona1BusinessKey);
        var persona2BusinessKey = BusinessKey.FromGuid(Persona2BusinessKey);
        var persona3BusinessKey = BusinessKey.FromGuid(Persona3BusinessKey);

        var personas = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = persona1BusinessKey,
                UserBusinessKey = user1BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                Name = "مدیریت اجرایی",
                OrganizationId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = persona2BusinessKey,
                UserBusinessKey = user2BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                Name = "کارشناس فنی",
                OrganizationId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = persona3BusinessKey,
                UserBusinessKey = user3BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                Name = "پشتیبانی مشتریان",
                OrganizationId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        };

        modelBuilder.Entity<OrganizationPersona>().HasData(personas);
    }

    private static void SeedRolePermissions(ModelBuilder modelBuilder)
    {
        var role1BusinessKey = BusinessKey.FromGuid(Role1BusinessKey);
        var role2BusinessKey = BusinessKey.FromGuid(Role2BusinessKey);
        var role3BusinessKey = BusinessKey.FromGuid(Role3BusinessKey);
        var permission1BusinessKey = BusinessKey.FromGuid(Permission1BusinessKey);
        var permission2BusinessKey = BusinessKey.FromGuid(Permission2BusinessKey);
        var permission3BusinessKey = BusinessKey.FromGuid(Permission3BusinessKey);

        // Use fixed GUIDs for RolePermission BusinessKeys from migration
        var rolePermissionBusinessKeys = new[]
        {
            Guid.Parse("6c9a4f5e-b015-41bb-b856-6f145fadf36a"),
            Guid.Parse("325c9c71-fd2e-4677-b625-6644fba715ea"),
            Guid.Parse("e37986ec-6c66-466c-ab2e-e3229ab333b4"),
            Guid.Parse("be447c89-637a-49b2-bce4-7030828b698a"),
            Guid.Parse("1dfea0a1-9bbf-4ed5-8345-8698747a03a7"),
            Guid.Parse("e7930cdf-4216-497a-8e2d-80512be89244")
        };

        var rolePermissions = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[0]),
                RoleBusinessKey = role1BusinessKey,
                PermissionBusinessKey = permission1BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[1]),
                RoleBusinessKey = role1BusinessKey,
                PermissionBusinessKey = permission2BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[2]),
                RoleBusinessKey = role1BusinessKey,
                PermissionBusinessKey = permission3BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 4L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[3]),
                RoleBusinessKey = role2BusinessKey,
                PermissionBusinessKey = permission2BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 5L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[4]),
                RoleBusinessKey = role2BusinessKey,
                PermissionBusinessKey = permission3BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 6L,
                BusinessKey = BusinessKey.FromGuid(rolePermissionBusinessKeys[5]),
                RoleBusinessKey = role3BusinessKey,
                PermissionBusinessKey = permission3BusinessKey,
                AppRoleId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        };

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }

    private static void SeedUserOrganizations(ModelBuilder modelBuilder)
    {
        var orgBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey);
        var user1BusinessKey = BusinessKey.FromGuid(User1BusinessKey);
        var user2BusinessKey = BusinessKey.FromGuid(User2BusinessKey);
        var user3BusinessKey = BusinessKey.FromGuid(User3BusinessKey);
        var persona2BusinessKey = BusinessKey.FromGuid(Persona2BusinessKey);
        var persona3BusinessKey = BusinessKey.FromGuid(Persona3BusinessKey);

        // Use fixed GUIDs for UserOrganization BusinessKeys from migration
        var userOrgBusinessKeys = new[]
        {
            Guid.Parse("f8ae7654-8945-461a-8422-0a592ba2ea48"),
            Guid.Parse("4f943333-0ead-494d-ad58-f21988ace7b4"),
            Guid.Parse("d976a82b-a137-4f51-89b1-7d995d40b7cb")
        };

        var userOrganizations = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = BusinessKey.FromGuid(userOrgBusinessKeys[0]),
                UserBusinessKey = user1BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                AssignmentType = AssignmentType.Direct,
                PersonaBusinessKey = (BusinessKey?)null,
                StartDate = FixedDate,
                EndDate = (DateTime?)null,
                IsActive = true,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = BusinessKey.FromGuid(userOrgBusinessKeys[1]),
                UserBusinessKey = user2BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                AssignmentType = AssignmentType.ViaPersona,
                PersonaBusinessKey = persona2BusinessKey,
                StartDate = FixedDate,
                EndDate = (DateTime?)null,
                IsActive = true,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = BusinessKey.FromGuid(userOrgBusinessKeys[2]),
                UserBusinessKey = user3BusinessKey,
                OrganizationBusinessKey = orgBusinessKey,
                AssignmentType = AssignmentType.ViaPersona,
                PersonaBusinessKey = persona3BusinessKey,
                StartDate = FixedDate,
                EndDate = (DateTime?)null,
                IsActive = true,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        };

        modelBuilder.Entity<UserOrganization>().HasData(userOrganizations);
    }

    private static void SeedUserPersonas(ModelBuilder modelBuilder)
    {
        var user1BusinessKey = BusinessKey.FromGuid(User1BusinessKey);
        var user2BusinessKey = BusinessKey.FromGuid(User2BusinessKey);
        var user3BusinessKey = BusinessKey.FromGuid(User3BusinessKey);
        var persona1BusinessKey = BusinessKey.FromGuid(Persona1BusinessKey);
        var persona2BusinessKey = BusinessKey.FromGuid(Persona2BusinessKey);
        var persona3BusinessKey = BusinessKey.FromGuid(Persona3BusinessKey);

        // Use fixed GUIDs for UserPersona BusinessKeys from migration
        var userPersonaBusinessKeys = new[]
        {
            Guid.Parse("ec2e2912-2758-4d64-a733-ed6d81eb9486"),
            Guid.Parse("0861b7ac-acf9-480e-84ec-552f0cc7609c"),
            Guid.Parse("f64d13c2-b06c-4a8a-a3bb-b38d248900e4")
        };

        var userPersonas = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = BusinessKey.FromGuid(userPersonaBusinessKeys[0]),
                UserBusinessKey = user1BusinessKey,
                PersonaBusinessKey = persona1BusinessKey,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = BusinessKey.FromGuid(userPersonaBusinessKeys[1]),
                UserBusinessKey = user2BusinessKey,
                PersonaBusinessKey = persona2BusinessKey,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = BusinessKey.FromGuid(userPersonaBusinessKeys[2]),
                UserBusinessKey = user3BusinessKey,
                PersonaBusinessKey = persona3BusinessKey,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        };

        modelBuilder.Entity<UserPersona>().HasData(userPersonas);
    }

    private static void SeedUserRoles(ModelBuilder modelBuilder)
    {
        var user1BusinessKey = BusinessKey.FromGuid(User1BusinessKey);
        var user2BusinessKey = BusinessKey.FromGuid(User2BusinessKey);
        var user3BusinessKey = BusinessKey.FromGuid(User3BusinessKey);
        var role1BusinessKey = BusinessKey.FromGuid(Role1BusinessKey);
        var role2BusinessKey = BusinessKey.FromGuid(Role2BusinessKey);
        var role3BusinessKey = BusinessKey.FromGuid(Role3BusinessKey);
        var persona1BusinessKey = BusinessKey.FromGuid(Persona1BusinessKey);
        var persona2BusinessKey = BusinessKey.FromGuid(Persona2BusinessKey);
        var persona3BusinessKey = BusinessKey.FromGuid(Persona3BusinessKey);

        // Use fixed GUIDs for UserRole BusinessKeys from migration
        var userRoleBusinessKeys = new[]
        {
            Guid.Parse("50485188-96c0-426e-bbf6-bc9742f07e67"),
            Guid.Parse("a2ff561d-745d-4778-8305-5c21240f9779"),
            Guid.Parse("fb9ff810-0cfe-45ab-9f4e-2477d736d926")
        };

        var userRoles = new[]
        {
            new
            {
                Id = 1L,
                BusinessKey = BusinessKey.FromGuid(userRoleBusinessKeys[0]),
                UserId = user1BusinessKey,
                RoleId = role1BusinessKey,
                PersonaId = persona1BusinessKey,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 2L,
                BusinessKey = BusinessKey.FromGuid(userRoleBusinessKeys[1]),
                UserId = user2BusinessKey,
                RoleId = role2BusinessKey,
                PersonaId = persona2BusinessKey,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            },
            new
            {
                Id = 3L,
                BusinessKey = BusinessKey.FromGuid(userRoleBusinessKeys[2]),
                UserId = user3BusinessKey,
                RoleId = role3BusinessKey,
                PersonaId = persona3BusinessKey,
                AppUserId = (long?)null,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }
        };

        modelBuilder.Entity<UserRole>().HasData(userRoles);
    }
}