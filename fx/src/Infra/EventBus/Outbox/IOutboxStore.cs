namespace OysterFx.Infra.EventBus.Outbox;

public interface IOutboxStore
{
    Task AddAsync(OutboxMessage message, CancellationToken ct);
    Task<IReadOnlyList<OutboxMessage>> GetUnpublishedAsync(int take, CancellationToken ct);
    Task MarkPublishedAsync(Guid id, CancellationToken ct);
}