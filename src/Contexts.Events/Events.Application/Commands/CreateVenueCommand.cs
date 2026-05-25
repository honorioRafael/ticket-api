namespace Events.Application.Commands;

public record CreateVenueCommand(string Name, string Address, int Capacity);
