namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands.Interceptors;
using OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;
using System.Globalization;

public class InsuranceUserServiceDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public InsuranceUserServiceDbContext(DbContextOptions<InsuranceUserServiceDbContext> options) : base(options) { }

    public InsuranceUserServiceDbContext() { }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationPersona> Personas { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPersona> UserPersonas { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public T? GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null
            ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    public object GetShadowPropertyValue(object entity, string propertyName)
    {
        return Entry(entity).Property(propertyName).CurrentValue!;
    }
    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }
            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }
                stack.Push(entityNavigations.GetEnumerator());
            }
            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();
            if (stack.Count == 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.AddAuditableShadowProperties();

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpirationDate);

            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.ReasonRevoked).HasMaxLength(500);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
        });

        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
        });

        builder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
        });

        builder.Entity<UserOrganization>(entity =>
        {
            entity.ToTable("UserOrganizations");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
        });

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
        });

        builder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
        });

        builder.Entity<UserPersona>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessKey).HasConversion(v => v.Value, v => BusinessKey.FromGuid(v)).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPersonas)
                .HasForeignKey(e => e.UserBusinessKey)
                .HasPrincipalKey(u => u.BusinessKey);

            entity.HasOne(e => e.Persona)
                .WithMany()
                .HasForeignKey(e => e.PersonaBusinessKey)
                .HasPrincipalKey(p => p.BusinessKey);
        });

        SeedData.Seed(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new AddAuditDataInterceptor());

        // Suppress the pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<BusinessKey>().HaveConversion<BusinessKeyConversion>();
    }
}