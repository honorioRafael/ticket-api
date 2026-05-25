using System;

namespace Events.Application.Commands;

public record CreateEventCommand(string Name, DateTime StartsAt, DateTime EndsAt, Guid VenueId);
