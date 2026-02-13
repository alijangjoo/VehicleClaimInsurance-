namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class OutboxDIExtensions
{
    public static IServiceCollection AddEventOutbox<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.AddScoped<IOutboxStore, EfCoreOutboxStore<TDbContext>>();
        services.AddHostedService<OutboxDispatcherBackgroundService>();

        return services;
    }
}