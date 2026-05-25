namespace Events.Application.Features.Venues.UpdateVenue;

public record UpdateVenueCommand(Guid Id, string Name, string Address, int Capacity);
