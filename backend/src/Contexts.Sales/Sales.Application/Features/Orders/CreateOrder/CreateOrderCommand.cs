namespace Sales.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(Guid CustomerId, List<OrderItemInput> Items);
