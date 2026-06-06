namespace Events.Application.DTOs;

public record TicketTypeDto(Guid Id, Guid EventId, string Name, decimal Price, int TotalQuantity, int AvailableQuantity);
