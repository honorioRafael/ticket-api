using AutoMapper;
using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Orders.CreateOrder;

public class CreateOrderUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IMapper _mapper;

    public CreateOrderUseCase(ICustomerRepository customerRepository, IOrderRepository orderRepository, IReservationRepository reservationRepository, ITicketTypeRepository ticketTypeRepository, IEventRepository eventRepository, IUnitOfWork unitOfWork, IValidator<CreateOrderCommand> validator, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _reservationRepository = reservationRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<OrderDto> ExecuteAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
            throw new CustomerNotFoundException();

        var orderItems = new List<(Guid TicketTypeId, decimal UnitPrice, int Quantity)>();
        var ticketTypesToUpdate = new List<(TicketType TicketType, int Quantity)>();

        var now = DateTime.UtcNow;

        foreach (var item in command.Items)
        {
            var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId, cancellationToken);
            if (ticketType == null)
                throw new InsufficientStockException($"Tipo de ingresso {item.TicketTypeId} não encontrado.");

            var @event = await _eventRepository.GetByIdAsync(ticketType.EventId, cancellationToken);
            if (@event == null || !@event.IsActive(now))
                throw new EventNotActiveException();

            // This will throw InsufficientStockException if stock < item.Quantity
            ticketType.DecrementStock(item.Quantity);

            orderItems.Add((ticketType.Id, ticketType.Price, item.Quantity));
            ticketTypesToUpdate.Add((ticketType, item.Quantity));
        }

        var order = new Order(command.CustomerId, orderItems);

        await _orderRepository.AddAsync(order, cancellationToken);

        foreach (var (ticketType, quantity) in ticketTypesToUpdate)
        {
            var reservation = new Reservation(order.Id, ticketType.Id, quantity, order.PlacedAt);
            await _reservationRepository.AddAsync(reservation, cancellationToken);
            _ticketTypeRepository.Update(ticketType);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }
}
