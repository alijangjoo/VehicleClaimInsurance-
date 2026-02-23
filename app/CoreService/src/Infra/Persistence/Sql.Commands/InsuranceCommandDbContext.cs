namespace Insurance.Infra.Persistence.Sql.Commands;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InsuranceCommandDbContext : CommandDbContext
{
    public InsuranceCommandDbContext(DbContextOptions<InsuranceCommandDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<Lost> Losts { get; set; }
}