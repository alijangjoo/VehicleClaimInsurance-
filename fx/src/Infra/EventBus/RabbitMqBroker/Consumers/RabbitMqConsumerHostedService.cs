namespace OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.RabbitMqBroker.Connections;

public sealed class RabbitMqConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public RabbitMqConsumerHostedService(IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection)
    {
        _serviceProvider = serviceProvider;
        _rabbitMqConnection = rabbitMqConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var registrar = _serviceProvider.GetRequiredService<IEventConsumerRegistrar>();
        registrar.RegisterConsumers();
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}