using System;
using System.Collections.Generic;

namespace Events.Application.DTOs;

public record VenueDto(Guid Id, string Name, string Address, int Capacity);

public record TicketTypeDto(Guid Id, Guid EventId, string Name, decimal Price, int TotalQuantity, int AvailableQuantity);

public record EventDto(
    Guid Id,
    string Name,
    DateTime StartsAt,
    DateTime EndsAt,
    string Status,
    Guid VenueId,
    IReadOnlyCollection<TicketTypeDto> TicketTypes
);
