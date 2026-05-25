using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Orders.DeleteOrder;

public class DeleteOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            throw new OrderNotFoundException();

        _orderRepository.Remove(order);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
