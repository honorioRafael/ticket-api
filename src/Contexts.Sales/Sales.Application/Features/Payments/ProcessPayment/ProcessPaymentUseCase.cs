using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Payments.ProcessPayment;

public class ProcessPaymentUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ProcessPaymentCommand> _validator;

    public ProcessPaymentUseCase(
        IOrderRepository orderRepository,
        IReservationRepository reservationRepository,
        IPaymentRepository paymentRepository,
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        IValidator<ProcessPaymentCommand> validator)
    {
        _orderRepository = orderRepository;
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<PaymentDto> ExecuteAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order == null)
            throw new OrderNotFoundException();

        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("O pagamento só pode ser processado para pedidos pendentes.");

        var reservations = await _reservationRepository.GetActiveReservationsByOrderIdAsync(order.Id, cancellationToken);
        var now = DateTime.UtcNow;

        if (!reservations.Any() || reservations.Any(r => r.ExpiresAt < now))
        {
            throw new ReservationExpiredException();
        }

        var methodCleaned = command.Method.Replace("_", "");
        if (!Enum.TryParse<PaymentMethod>(methodCleaned, true, out var paymentMethod))
        {
            throw new ArgumentException("Método de pagamento inválido.", nameof(command.Method));
        }

        var payment = new Payment(order.Id, paymentMethod, order.TotalAmount);
        payment.Pay();

        order.Confirm();
        foreach (var reservation in reservations)
        {
            reservation.Confirm();
            _reservationRepository.Update(reservation);
        }

        var tickets = new List<Ticket>();
        foreach (var item in order.OrderItems)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                var ticketCode = $"TKT-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
                var ticket = new Ticket(item.Id, ticketCode);
                tickets.Add(ticket);
            }
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _ticketRepository.AddRangeAsync(tickets, cancellationToken);
        _orderRepository.Update(order);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            command.Method.ToLower(),
            payment.Status.ToString().ToLower(),
            payment.Amount,
            payment.PaidAt
        );
    }
}
