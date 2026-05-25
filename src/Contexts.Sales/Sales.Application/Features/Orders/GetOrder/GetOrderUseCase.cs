using Sales.Application.DTOs;
using Sales.Application.Features.Orders.CreateOrder;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Orders.GetOrder;

public class GetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new OrderNotFoundException();

        return CreateOrderUseCase.MapToDto(order);
    }
}
