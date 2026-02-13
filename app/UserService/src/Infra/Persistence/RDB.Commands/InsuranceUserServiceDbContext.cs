namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;

public class InsuranceUserServiceDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationPersona> Personas { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPersona> UserPersonas { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<IdentityUserRole<long>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessKey)
                .HasConversion(
                    v => v.Value,
                    v => BusinessKey.FromGuid(v))
                .IsRequired();
        });

        builder.Entity<UserPersona>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPersonas)
                .HasForeignKey(e => e.UserBusinessKey)
                .HasPrincipalKey(u => u.BusinessKey);

            entity.HasOne(e => e.Persona)
                .WithMany()
                .HasForeignKey(e => e.PersonaBusinessKey)
                .HasPrincipalKey(p => p.BusinessKey);
        });
    }
}