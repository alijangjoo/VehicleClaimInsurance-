namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.EntityFrameworkCore;

public sealed class EfCoreOutboxStore<TDbContext> : IOutboxStore where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EfCoreOutboxStore(TDbContext dbContext) => _dbContext = dbContext;

    public Task AddAsync(OutboxMessage message, CancellationToken ct)
    {
        _dbContext.Set<OutboxMessage>().Add(message);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnpublishedAsync(int take, CancellationToken ct)
    {
        return await _dbContext.Set<OutboxMessage>()
            .Where(x => !x.Published)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task MarkPublishedAsync(Guid id, CancellationToken ct)
    {
        var msg = await _dbContext.Set<OutboxMessage>().FindAsync(id, ct);
        if (msg is null) return;

        msg.Published = true;
        await _dbContext.SaveChangesAsync(ct);
    }
}