using System;

namespace Events.Application.Commands;

public record CreateTicketTypeCommand(Guid EventId, string Name, decimal Price, int TotalQuantity);
