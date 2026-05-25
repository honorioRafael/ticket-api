namespace Events.Application.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    DateTime StartsAt,
    DateTime EndsAt,
    string Status,
    Guid VenueId,
    IReadOnlyCollection<TicketTypeDto> TicketTypes
);
