using Sales.Application.Features.Orders.CreateOrder;

namespace Sales.API.Requests;

public record CreateOrderRequest(List<OrderItemInput> Items);
