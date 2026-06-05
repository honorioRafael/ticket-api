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

    public async Task ExecuteAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, customerId, cancellationToken);
        if (order == null)
            throw new DomainException(DomainErrorCode.NotFound, "Pedido não encontrado.");

        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}
