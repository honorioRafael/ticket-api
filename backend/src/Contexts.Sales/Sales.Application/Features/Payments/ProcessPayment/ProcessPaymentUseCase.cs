using FluentValidation;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Domain.Services;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Payments.ProcessPayment;

public class ProcessPaymentUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailQueueService _emailQueue;
    private readonly IValidator<ProcessPaymentCommand> _validator;

    public ProcessPaymentUseCase(IOrderRepository orderRepository, IPaymentRepository paymentRepository, ITicketRepository ticketRepository, ICustomerRepository customerRepository, IEmailQueueService emailQueue, IValidator<ProcessPaymentCommand> validator)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _customerRepository = customerRepository;
        _emailQueue = emailQueue;
        _validator = validator;
    }

    public async Task<PaymentDto> ExecuteAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orderRepository.GetByIdAsync(command.OrderId, command.CustomerId, cancellationToken);
        if (order == null)
            throw new DomainException(DomainErrorCode.NotFound, "Pedido não encontrado.");

        if (order.Status != OrderStatus.Pending)
            throw new DomainException(DomainErrorCode.RuleViolation, "O pagamento só pode ser processado para pedidos pendentes.");

        // Realiza o parse do método de pagamento.
        var methodCleaned = command.Method.Replace("_", "");
        if (!Enum.TryParse<PaymentMethod>(methodCleaned, true, out var paymentMethod))
        {
            throw new DomainException(DomainErrorCode.ValidationError, "Método de pagamento inválido.");
        }

        var payment = new Payment(order.Id, paymentMethod, order.TotalAmount);
        payment.Pay();

        order.Confirm();

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

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        if (customer != null)
        {
            await _emailQueue.PublishAsync(new EmailMessage(
                customer.Email.Value,
                customer.Name,
                order.Id,
                order.TotalAmount,
                tickets.Count
            ), cancellationToken);
        }

        return new PaymentDto(
            payment.Id,
            payment.OrderId,
            command.Method.ToLower(),
            payment.Status.ToString().ToLower(),
            payment.Amount,
            payment.PaidAt,
            tickets.Select(t => t.Code).ToList()
        );
    }
}
