namespace Insurance.Infra.Persistence.Sql.Commands;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InsuranceCommandDbContext : CommandDbContext
{
    public DbSet<Lost> Losts { get; set; }
}