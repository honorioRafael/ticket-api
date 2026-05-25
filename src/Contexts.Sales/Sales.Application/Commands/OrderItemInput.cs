using System;

namespace Sales.Application.Commands;

public record OrderItemInput(Guid TicketTypeId, int Quantity);
