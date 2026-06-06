namespace Events.Application.Features.Venues.CreateVenue;

public record CreateVenueCommand(string Name, string Address, int Capacity);
