namespace OysterFx.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using OysterFx.Infra.Persistence.RDB.Commands.Configs;
using OysterFx.Infra.Persistence.RDB.Commands.Interceptors;

public abstract class CommandWithEventSourcingDbContext : CommandDbContext
{
    public DbSet<OutboxEvent> Events { get; set; }

    protected CommandWithEventSourcingDbContext() { }
    protected CommandWithEventSourcingDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new AddAuditDataInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new OutboxEventConfig());
    }
}