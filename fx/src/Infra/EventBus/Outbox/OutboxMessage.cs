namespace OysterFx.Infra.EventBus.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string EventType { get; init; } = default!;
    public string Payload { get; init; } = default!;
    public string Metadata { get; init; } = default!;
    public int RetryCount { get; set; }
    public bool Published { get; set; }
    public DateTime OccurredAtUtc { get; init; }
}