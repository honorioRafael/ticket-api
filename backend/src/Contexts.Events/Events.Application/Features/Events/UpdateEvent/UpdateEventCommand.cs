namespace Events.Application.Features.Events.UpdateEvent;

public record UpdateEventCommand(Guid Id, string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);
