using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<byte>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReasonRevoked = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Province = table.Column<string>(type: "text", nullable: true),
                    MaxAllowedExpertiseAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAllowedDailyCaseReferral = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAllowedOpenCases = table.Column<decimal>(type: "numeric", nullable: false),
                    GPSDeviceId = table.Column<string>(type: "text", nullable: true),
                    MobileNumber = table.Column<string>(type: "text", nullable: true),
                    ProfileImageBase64 = table.Column<string>(type: "text", nullable: true),
                    UserExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_BusinessKey", x => x.BusinessKey);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationId = table.Column<long>(type: "bigint", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.UniqueConstraint("AK_Personas_BusinessKey", x => x.BusinessKey);
                    table.ForeignKey(
                        name: "FK_Personas_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    AppRoleId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<int>(type: "integer", nullable: false),
                    PersonaBusinessKey = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AppUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPersonas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPersonas_Personas_PersonaBusinessKey",
                        column: x => x.PersonaBusinessKey,
                        principalTable: "Personas",
                        principalColumn: "BusinessKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPersonas_Users_UserBusinessKey",
                        column: x => x.UserBusinessKey,
                        principalTable: "Users",
                        principalColumn: "BusinessKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "IsActive", "ModifiedByUserId", "ModifiedDateTime", "Name" },
                values: new object[] { 1L, new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), null, null, true, null, null, "شرکت نمونه داده‌پرداز" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "BusinessKey", "Code", "CreatedByUserId", "CreatedDateTime", "Description", "IsActive", "ModifiedByUserId", "ModifiedDateTime", "Scope" },
                values: new object[,]
                {
                    { 1L, new Guid("3dd4ec06-a23c-41fd-acc2-62ffe2bc7ea6"), "USER_CREATE", null, null, "ایجاد کاربر جدید", true, null, null, (byte)1 },
                    { 2L, new Guid("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7"), "Losts.AssignHullThirdPartyInsurerCode", null, null, "صدور کد بیمه گران طرف قرارداد بدنه", true, null, null, (byte)1 },
                    { 3L, new Guid("82514bf2-ebb6-4e75-84a5-9c4d8831121d"), "Losts.AssignThirdPartyInsurerCode", null, null, "صدور کد بیمه گران طرف قرارداد ثالث مالی", true, null, null, (byte)1 },
                    { 4L, new Guid("874b7fe9-b8b6-41f5-b00c-b611ede1634e"), "ROLE_ASSIGN", null, null, "اختصاص نقش به کاربران", true, null, null, (byte)2 }
                });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "ModifiedByUserId", "ModifiedDateTime", "Name", "OrganizationBusinessKey", "OrganizationId", "UserBusinessKey" },
                values: new object[,]
                {
                    { 1L, new Guid("eb1598b6-8a1c-445f-aab2-208aad4941b6"), null, null, null, null, "مدیریت اجرایی", new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), null, new Guid("ef58349f-d52c-4c5c-9966-0ec7c14f3c51") },
                    { 2L, new Guid("c7b76748-07c3-4030-98fc-c1740002c00e"), null, null, null, null, "کارشناس فنی", new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), null, new Guid("0bb85299-367b-4d68-8db1-5e66003a5522") },
                    { 3L, new Guid("99ff2684-86d1-45d4-9df8-e1a982083bcc"), null, null, null, null, "پشتیبانی مشتریان", new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), null, new Guid("9e500e63-7399-4afa-87e2-cc7f5e1f9065") }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AppRoleId", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "ModifiedByUserId", "ModifiedDateTime", "PermissionBusinessKey", "RoleBusinessKey" },
                values: new object[,]
                {
                    { 1L, null, new Guid("6c9a4f5e-b015-41bb-b856-6f145fadf36a"), null, null, null, null, new Guid("3dd4ec06-a23c-41fd-acc2-62ffe2bc7ea6"), new Guid("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96") },
                    { 2L, null, new Guid("325c9c71-fd2e-4677-b625-6644fba715ea"), null, null, null, null, new Guid("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7"), new Guid("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96") },
                    { 3L, null, new Guid("e37986ec-6c66-466c-ab2e-e3229ab333b4"), null, null, null, null, new Guid("82514bf2-ebb6-4e75-84a5-9c4d8831121d"), new Guid("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96") },
                    { 4L, null, new Guid("be447c89-637a-49b2-bce4-7030828b698a"), null, null, null, null, new Guid("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7"), new Guid("17772f1c-d812-4a18-b30e-a30068d8ca57") },
                    { 5L, null, new Guid("1dfea0a1-9bbf-4ed5-8345-8698747a03a7"), null, null, null, null, new Guid("82514bf2-ebb6-4e75-84a5-9c4d8831121d"), new Guid("17772f1c-d812-4a18-b30e-a30068d8ca57") },
                    { 6L, null, new Guid("e7930cdf-4216-497a-8e2d-80512be89244"), null, null, null, null, new Guid("82514bf2-ebb6-4e75-84a5-9c4d8831121d"), new Guid("3ee32a19-e5f1-468d-baf1-321761ed8b60") }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "BusinessKey", "ConcurrencyStamp", "Name", "NormalizedName", "Scope" },
                values: new object[,]
                {
                    { 1L, new Guid("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96"), null, "مدیر سیستم", "مدیر سیستم", (byte)1 },
                    { 2L, new Guid("17772f1c-d812-4a18-b30e-a30068d8ca57"), null, "مدیر سازمان", "مدیر سازمان", (byte)2 },
                    { 3L, new Guid("3ee32a19-e5f1-468d-baf1-321761ed8b60"), null, "کاربر عادی", "کاربر عادی", (byte)3 }
                });

            migrationBuilder.InsertData(
                table: "UserOrganizations",
                columns: new[] { "Id", "AppUserId", "AssignmentType", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "EndDate", "IsActive", "ModifiedByUserId", "ModifiedDateTime", "OrganizationBusinessKey", "PersonaBusinessKey", "StartDate", "UserBusinessKey" },
                values: new object[,]
                {
                    { 1L, null, 1, new Guid("f8ae7654-8945-461a-8422-0a592ba2ea48"), null, null, null, true, null, null, new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), null, new DateTime(2026, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), new Guid("ef58349f-d52c-4c5c-9966-0ec7c14f3c51") },
                    { 2L, null, 2, new Guid("4f943333-0ead-494d-ad58-f21988ace7b4"), null, null, null, true, null, null, new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), new Guid("c7b76748-07c3-4030-98fc-c1740002c00e"), new DateTime(2026, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), new Guid("0bb85299-367b-4d68-8db1-5e66003a5522") },
                    { 3L, null, 2, new Guid("d976a82b-a137-4f51-89b1-7d995d40b7cb"), null, null, null, true, null, null, new Guid("27632bf0-4ab2-4aa6-a606-b1e5d0135258"), new Guid("99ff2684-86d1-45d4-9df8-e1a982083bcc"), new DateTime(2026, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), new Guid("9e500e63-7399-4afa-87e2-cc7f5e1f9065") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "AppUserId", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "ModifiedByUserId", "ModifiedDateTime", "PersonaId", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1L, null, new Guid("50485188-96c0-426e-bbf6-bc9742f07e67"), null, null, null, null, new Guid("eb1598b6-8a1c-445f-aab2-208aad4941b6"), new Guid("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96"), new Guid("ef58349f-d52c-4c5c-9966-0ec7c14f3c51") },
                    { 2L, null, new Guid("a2ff561d-745d-4778-8305-5c21240f9779"), null, null, null, null, new Guid("c7b76748-07c3-4030-98fc-c1740002c00e"), new Guid("17772f1c-d812-4a18-b30e-a30068d8ca57"), new Guid("0bb85299-367b-4d68-8db1-5e66003a5522") },
                    { 3L, null, new Guid("fb9ff810-0cfe-45ab-9f4e-2477d736d926"), null, null, null, null, new Guid("99ff2684-86d1-45d4-9df8-e1a982083bcc"), new Guid("3ee32a19-e5f1-468d-baf1-321761ed8b60"), new Guid("9e500e63-7399-4afa-87e2-cc7f5e1f9065") }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "BusinessKey", "City", "Code", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "GPSDeviceId", "LockoutEnabled", "LockoutEnd", "MaxAllowedDailyCaseReferral", "MaxAllowedExpertiseAmount", "MaxAllowedOpenCases", "MobileNumber", "NormalizedEmail", "NormalizedUserName", "PasswordExpirationDate", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImageBase64", "Province", "SecurityStamp", "TwoFactorEnabled", "UserExpirationDate", "UserName" },
                values: new object[,]
                {
                    { 1L, 0, new Guid("ef58349f-d52c-4c5c-9966-0ec7c14f3c51"), "تهران", "AD001", "fa2c7f2e-e506-4154-bcfb-d2f11e4bdeeb", "admin@example.com", true, "مدیر ارشد", "GPS001", true, null, 50m, 10000000m, 100m, "09121111111", "ADMIN@EXAMPLE.COM", "ADMIN", new DateTime(2026, 5, 23, 22, 5, 5, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEI...", null, true, null, "تهران", "30d09939-a2ab-4840-afe8-5eb302182d21", false, new DateTime(2027, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), "admin" },
                    { 2L, 0, new Guid("0bb85299-367b-4d68-8db1-5e66003a5522"), "تهران", "EX001", "a217f7eb-5508-4e02-8566-95a4a1bdab7c", "expert@example.com", true, "کارشناس فنی", "GPS002", true, null, 30m, 5000000m, 50m, "09122222222", "EXPERT@EXAMPLE.COM", "EXPERT", new DateTime(2026, 5, 23, 22, 5, 5, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEI...", null, true, null, "تهران", "60d0bb73-efe5-4e30-8dcf-2827606b892d", false, new DateTime(2027, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), "expert" },
                    { 3L, 0, new Guid("9e500e63-7399-4afa-87e2-cc7f5e1f9065"), "تهران", "SU001", "f1283964-4378-45f5-8815-57f1ce714caa", "support@example.com", true, "پشتیبان", "GPS003", true, null, 20m, 0m, 30m, "09123333333", "SUPPORT@EXAMPLE.COM", "SUPPORT", new DateTime(2026, 5, 23, 22, 5, 5, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEI...", null, true, null, "تهران", "0653d8d2-7653-45d7-8215-c07526e07404", false, new DateTime(2027, 2, 23, 22, 5, 5, 0, DateTimeKind.Utc), "support" }
                });

            migrationBuilder.InsertData(
                table: "UserPersonas",
                columns: new[] { "Id", "BusinessKey", "CreatedByUserId", "CreatedDateTime", "ModifiedByUserId", "ModifiedDateTime", "PersonaBusinessKey", "UserBusinessKey" },
                values: new object[,]
                {
                    { 1L, new Guid("ec2e2912-2758-4d64-a733-ed6d81eb9486"), null, null, null, null, new Guid("eb1598b6-8a1c-445f-aab2-208aad4941b6"), new Guid("ef58349f-d52c-4c5c-9966-0ec7c14f3c51") },
                    { 2L, new Guid("0861b7ac-acf9-480e-84ec-552f0cc7609c"), null, null, null, null, new Guid("c7b76748-07c3-4030-98fc-c1740002c00e"), new Guid("0bb85299-367b-4d68-8db1-5e66003a5522") },
                    { 3L, new Guid("f64d13c2-b06c-4a8a-a3bb-b38d248900e4"), null, null, null, null, new Guid("99ff2684-86d1-45d4-9df8-e1a982083bcc"), new Guid("9e500e63-7399-4afa-87e2-cc7f5e1f9065") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_OrganizationId",
                table: "Personas",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpirationDate",
                table: "RefreshTokens",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_AppRoleId",
                table: "RolePermissions",
                column: "AppRoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_AppUserId",
                table: "UserOrganizations",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonas_PersonaBusinessKey",
                table: "UserPersonas",
                column: "PersonaBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserPersonas_UserBusinessKey",
                table: "UserPersonas",
                column: "UserBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AppUserId",
                table: "UserRoles",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "UserPersonas");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
