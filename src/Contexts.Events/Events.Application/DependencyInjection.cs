using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Events.Application.UseCases;

namespace Events.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CreateVenueUseCase>();
        services.AddScoped<CreateEventUseCase>();
        services.AddScoped<CreateTicketTypeUseCase>();
        services.AddScoped<PublishEventUseCase>();
        services.AddScoped<CancelEventUseCase>();
        services.AddScoped<GetEventUseCase>();
        services.AddScoped<GetVenueUseCase>();

        return services;
    }
}
