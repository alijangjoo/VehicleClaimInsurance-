namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OysterFx.Infra.EventBus.Abstractions;
using System.Text.Json;

public sealed class OutboxDispatcherBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventBus _eventBus;

    public OutboxDispatcherBackgroundService(IServiceScopeFactory serviceScopeFactory, IEventBus eventBus)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var outboxStore = scope.ServiceProvider.GetRequiredService<IOutboxStore>();

            var messages = await outboxStore.GetUnpublishedAsync(20, stoppingToken);

            foreach (var msg in messages)
            {
                await _eventBus.PublishRawAsync(
                    msg.Id,
                    msg.EventType,
                    msg.Payload,
                    JsonSerializer.Deserialize<Dictionary<string, string>>(msg.Metadata)!,
                    stoppingToken);

                await outboxStore.MarkPublishedAsync(msg.Id, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}