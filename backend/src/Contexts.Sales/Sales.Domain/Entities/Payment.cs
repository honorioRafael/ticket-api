using Sales.Domain.Enums;
using TicketApi.Common.Exceptions;

namespace Sales.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Payment() { }

    public Payment(Guid orderId, PaymentMethod method, decimal amount)
    {
        if (amount <= 0)
            throw new DomainException(DomainErrorCode.ValidationError, "O valor deve ser maior que zero.");

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        Method = method;
        Status = PaymentStatus.Pending;
        Amount = amount;
    }

    public void Pay()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(DomainErrorCode.RuleViolation, $"Não é possível processar o pagamento com status {Status}.");

        Status = PaymentStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(DomainErrorCode.RuleViolation, $"Não é possível marcar como falho o pagamento com status {Status}.");

        Status = PaymentStatus.Failed;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Paid)
            throw new DomainException(DomainErrorCode.RuleViolation, $"Não é possível estornar o pagamento com status {Status}.");

        Status = PaymentStatus.Refunded;
    }
}
