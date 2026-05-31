using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Orders.DeleteOrder;

public class DeleteOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new DomainException("ORDER_NOT_FOUND", "Pedido não encontrado.");

        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}
