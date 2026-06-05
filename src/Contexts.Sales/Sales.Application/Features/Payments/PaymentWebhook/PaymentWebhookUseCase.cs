using FluentValidation;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Domain.Services;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Payments.PaymentWebhook;

public class PaymentWebhookUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailQueueService _emailQueue;
    private readonly IValidator<PaymentWebhookCommand> _validator;

    public PaymentWebhookUseCase(IOrderRepository orderRepository, IPaymentRepository paymentRepository, ITicketRepository ticketRepository, ITicketTypeRepository ticketTypeRepository, ICustomerRepository customerRepository, IEmailQueueService emailQueue, IValidator<PaymentWebhookCommand> validator)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _customerRepository = customerRepository;
        _emailQueue = emailQueue;
        _validator = validator;
    }

    public async Task ExecuteAsync(PaymentWebhookCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var order = await _orderRepository.GetByIdAsync(command.OrderId, command.CustomerId, cancellationToken);
        if (order == null)
            throw new DomainException(DomainErrorCode.NotFound, "Pedido não encontrado.");

        // Se o pedido não estiver pendente, o pagamento já foi processado ou o pedido foi cancelado.
        if (order.Status != OrderStatus.Pending)
        {
            return;
        }

        // Realiza o parse do status de pagamento vindo no webhook.
        if (!Enum.TryParse<PaymentStatus>(command.Status, true, out var paymentStatus) ||
            (paymentStatus != PaymentStatus.Paid && paymentStatus != PaymentStatus.Failed))
        {
            throw new DomainException(DomainErrorCode.ValidationError, "Status de pagamento inválido. Apenas 'paid' ou 'failed' são aceitos.");
        }

        // Realiza o parse do método de pagamento.
        var methodCleaned = command.Method.Replace("_", "");
        if (!Enum.TryParse<PaymentMethod>(methodCleaned, true, out var paymentMethod))
        {
            throw new DomainException(DomainErrorCode.ValidationError, "Método de pagamento inválido.");
        }

        var payment = new Payment(order.Id, paymentMethod, order.TotalAmount);

        if (paymentStatus == PaymentStatus.Paid)
        {
            payment.Pay();
            order.Confirm();

            var tickets = new List<Ticket>();
            foreach (var item in order.OrderItems)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    var ticketCode = $"TKT-{Guid.CreateVersion7().ToString("N").Substring(0, 12).ToUpper()}";
                    var ticket = new Ticket(item.Id, ticketCode);
                    tickets.Add(ticket);
                }
            }

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _ticketRepository.AddRangeAsync(tickets, cancellationToken);

            // Publica na fila SQS para envio de e-mail de confirmação
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
        }
        else // paymentStatus == PaymentStatus.Failed
        {
            payment.Fail();
            order.Cancel();

            foreach (var item in order.OrderItems)
            {
                var ticketType = await _ticketTypeRepository.GetByIdAsync(item.TicketTypeId, cancellationToken);
                if (ticketType != null)
                {
                    ticketType.IncrementAvailableQuantity(item.Quantity);
                }
            }

            await _paymentRepository.AddAsync(payment, cancellationToken);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}
