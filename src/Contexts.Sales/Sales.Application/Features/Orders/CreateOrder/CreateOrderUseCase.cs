using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Events.Domain.Entities;
using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Orders.CreateOrder;

public class CreateOrderUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IMapper _mapper;

    public CreateOrderUseCase(ICustomerRepository customerRepository, IOrderRepository orderRepository, ITicketTypeRepository ticketTypeRepository, IEventRepository eventRepository, IValidator<CreateOrderCommand> validator, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _eventRepository = eventRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<OrderDto> ExecuteAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
            throw new DomainException(DomainErrorCode.NotFound, "Cliente não encontrado.");

        var orderItems = new List<(Guid TicketTypeId, decimal UnitPrice, int Quantity)>();
        var ticketTypesToUpdate = new List<(TicketType TicketType, int Quantity)>();

        var now = DateTime.UtcNow;

        foreach (var item in command.Items)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId, cancellationToken);
            if (ticketType == null)
                throw new DomainException(DomainErrorCode.NotFound, $"Tipo de ingresso {item.TicketTypeId} não encontrado.");

            var @event = await _eventRepository.GetByIdAsync(ticketType.EventId, cancellationToken);
            if (@event == null || !@event.IsActive(now))
                throw new DomainException(DomainErrorCode.RuleViolation, "O evento não está ativo para vendas.");

            if (ticketType.AvailableQuantity < item.Quantity)
                throw new DomainException(DomainErrorCode.RuleViolation, "Estoque de ingressos insuficiente.");

            ticketType.DecrementAvailableQuantity(item.Quantity);

            orderItems.Add((ticketType.Id, ticketType.Price, item.Quantity));
            ticketTypesToUpdate.Add((ticketType, item.Quantity));
        }

        var order = new Order(command.CustomerId, orderItems);

        await _orderRepository.AddAsync(order, cancellationToken);

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }
}
