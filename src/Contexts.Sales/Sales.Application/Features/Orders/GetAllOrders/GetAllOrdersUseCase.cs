using AutoMapper;
using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Models;

namespace Sales.Application.Features.Orders.GetAllOrders;

public class GetAllOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetAllOrdersUseCase(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderDto>> ExecuteAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) = await _orderRepository.GetAllAsync(page, pageSize, cancellationToken);

        var dtos = _mapper.Map<List<OrderDto>>(items);

        return new PaginatedList<OrderDto>(dtos, page, pageSize, totalCount);
    }
}
