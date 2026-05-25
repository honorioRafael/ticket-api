namespace Sales.Application.Features.Orders.CreateOrder;

public record OrderItemInput(Guid TicketTypeId, int Quantity);
