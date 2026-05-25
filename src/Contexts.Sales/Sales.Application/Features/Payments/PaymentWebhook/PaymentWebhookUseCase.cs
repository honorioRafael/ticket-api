using FluentValidation;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.Features.Payments.PaymentWebhook;

public class PaymentWebhookUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<PaymentWebhookCommand> _validator;

    public PaymentWebhookUseCase(IOrderRepository orderRepository, IReservationRepository reservationRepository, IPaymentRepository paymentRepository, ITicketRepository ticketRepository, ITicketTypeRepository ticketTypeRepository, IUnitOfWork unitOfWork, IValidator<PaymentWebhookCommand> validator)
    {
        _orderRepository = orderRepository;
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task ExecuteAsync(PaymentWebhookCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order == null)
            throw new OrderNotFoundException();

        if (order.Status != OrderStatus.Pending)
        {
            return;
        }

        var reservations = await _reservationRepository.GetActiveReservationsByOrderIdAsync(order.Id, cancellationToken);

        var methodCleaned = command.Method.Replace("_", "");
        if (!Enum.TryParse<PaymentMethod>(methodCleaned, true, out var paymentMethod))
        {
            throw new ArgumentException("Método de pagamento inválido.", nameof(command.Method));
        }

        if (command.Status.Equals("paid", StringComparison.OrdinalIgnoreCase))
        {
            var now = DateTime.UtcNow;
            if (!reservations.Any() || reservations.Any(r => r.ExpiresAt < now))
            {
                order.Cancel();
                foreach (var res in reservations)
                {
                    res.Expire();
                    _reservationRepository.Update(res);

                    var ticketType = await _ticketTypeRepository.GetByIdAsync(res.TicketTypeId, cancellationToken);
                    if (ticketType != null)
                    {
                        ticketType.RestoreStock(res.Quantity);
                        _ticketTypeRepository.Update(ticketType);
                    }
                }

                var paymentFailed = new Payment(order.Id, paymentMethod, order.TotalAmount);
                paymentFailed.Fail();
                await _paymentRepository.AddAsync(paymentFailed, cancellationToken);
                _orderRepository.Update(order);
                await _unitOfWork.CommitAsync(cancellationToken);
                throw new ReservationExpiredException("A reserva expirou. Pagamento marcado como falho.");
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
        }
        else
        {
            var payment = new Payment(order.Id, paymentMethod, order.TotalAmount);
            payment.Fail();

            order.Cancel();
            foreach (var reservation in reservations)
            {
                reservation.Cancel();
                _reservationRepository.Update(reservation);

                var ticketType = await _ticketTypeRepository.GetByIdAsync(reservation.TicketTypeId, cancellationToken);
                if (ticketType != null)
                {
                    ticketType.RestoreStock(reservation.Quantity);
                    _ticketTypeRepository.Update(ticketType);
                }
            }

            await _paymentRepository.AddAsync(payment, cancellationToken);
        }

        _orderRepository.Update(order);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
