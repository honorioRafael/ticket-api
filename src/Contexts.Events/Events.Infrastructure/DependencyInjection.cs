using Events.Domain.Repositories;
using Events.Infrastructure.Contexts;
using Events.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EventsDbContext>(options =>
            options.UseNpgsql(connectionString, o =>
                o.MigrationsHistoryTable("__EFMigrationsHistory")));

        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}
