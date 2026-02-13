namespace OysterFx.Infra.EventBus.RabbitMqBroker;

using Microsoft.Extensions.DependencyInjection;
using OysterFx.Infra.EventBus.Abstractions;
using OysterFx.Infra.EventBus.RabbitMqBroker.Consumers;

public static class ConsumerServiceDIExtensions
{
    public static IServiceCollection AddMessageConsumers<TRegistrar>(this IServiceCollection services) where TRegistrar : class, IEventConsumerRegistrar
    {
        services.AddSingleton<IEventConsumerRegistrar, TRegistrar>();
        services.AddHostedService<RabbitMqConsumerHostedService>();
        return services;
    }
}