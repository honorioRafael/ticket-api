using Sales.Application.DTOs;
using Sales.Application.Features.Orders.CreateOrder;
using Sales.Domain.Repositories;
using SharedKernel.Models;

namespace Sales.Application.Features.Orders.GetAllOrders;

public class GetAllOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedList<OrderDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _orderRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = items.Select(CreateOrderUseCase.MapToDto).ToList();

        return new PaginatedList<OrderDto>(dtos, page, pageSize, totalCount);
    }
}
