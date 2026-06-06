using AutoMapper;
using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Orders.GetOrder;

public class GetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderUseCase(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> ExecuteAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, customerId, cancellationToken);
        if (order == null)
            throw new DomainException(DomainErrorCode.NotFound, "Pedido não encontrado.");

        return _mapper.Map<OrderDto>(order);
    }
}
