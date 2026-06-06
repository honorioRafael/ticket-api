namespace Events.Application.Features.Events.CreateTicketType;

public record CreateTicketTypeCommand(Guid EventId, string Name, decimal Price, int TotalQuantity);
