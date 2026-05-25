using Events.Application.Features.Events.CancelEvent;
using Events.Application.Features.Events.CreateEvent;
using Events.Application.Features.Events.CreateTicketType;
using Events.Application.Features.Events.DeleteEvent;
using Events.Application.Features.Events.GetAllEvents;
using Events.Application.Features.Events.GetEvent;
using Events.Application.Features.Events.PublishEvent;
using Events.Application.Features.Events.UpdateEvent;
using Events.Application.Features.Venues.CreateVenue;
using Events.Application.Features.Venues.DeleteVenue;
using Events.Application.Features.Venues.GetAllVenues;
using Events.Application.Features.Venues.GetVenue;
using Events.Application.Features.Venues.UpdateVenue;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddEventsApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CreateVenueUseCase>();
        services.AddScoped<GetVenueUseCase>();
        services.AddScoped<UpdateVenueUseCase>();
        services.AddScoped<DeleteVenueUseCase>();
        services.AddScoped<GetAllVenuesUseCase>();

        services.AddScoped<CreateEventUseCase>();
        services.AddScoped<GetEventUseCase>();
        services.AddScoped<UpdateEventUseCase>();
        services.AddScoped<DeleteEventUseCase>();
        services.AddScoped<GetAllEventsUseCase>();
        services.AddScoped<PublishEventUseCase>();
        services.AddScoped<CancelEventUseCase>();
        services.AddScoped<CreateTicketTypeUseCase>();

        return services;
    }
}
