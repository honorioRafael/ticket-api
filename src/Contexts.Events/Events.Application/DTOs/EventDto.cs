namespace Events.Application.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    DateTime StartsAt,
    DateTime EndsAt,
    Guid VenueId,
    IReadOnlyCollection<TicketTypeDto> TicketTypes
);
