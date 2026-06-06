namespace Events.API.Requests;

public record UpdateEventRequest(string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);
