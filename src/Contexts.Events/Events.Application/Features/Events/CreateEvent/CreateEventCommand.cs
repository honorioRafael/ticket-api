namespace Events.Application.Features.Events.CreateEvent;

public record CreateEventCommand(string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);
