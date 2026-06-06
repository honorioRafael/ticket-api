using Sales.Domain.Enums;
using TicketApi.Common.Exceptions;

namespace Sales.Domain.Entities;

public class Ticket
{
    public Guid Id { get; private set; }
    public Guid OrderItemId { get; private set; }
    public string Code { get; private set; } = null!;
    public TicketStatus Status { get; private set; }

    private Ticket() { }

    public Ticket(Guid orderItemId, string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(DomainErrorCode.ValidationError, "O código não pode ser vazio.");

        Id = Guid.CreateVersion7();
        OrderItemId = orderItemId;
        Code = code;
        Status = TicketStatus.Active;
    }

    public void Use()
    {
        if (Status == TicketStatus.Used)
            throw new DomainException(DomainErrorCode.RuleViolation, "O ingresso já foi utilizado.");
        if (Status == TicketStatus.Cancelled)
            throw new DomainException(DomainErrorCode.RuleViolation, "O ingresso está cancelado.");
        if (Status != TicketStatus.Active)
            throw new DomainException(DomainErrorCode.RuleViolation, $"Não é possível utilizar o ingresso a partir do status {Status}.");

        Status = TicketStatus.Used;
    }

    public void Cancel()
    {
        if (Status != TicketStatus.Active)
            throw new DomainException(DomainErrorCode.RuleViolation, $"Não é possível cancelar o ingresso a partir do status {Status}.");

        Status = TicketStatus.Cancelled;
    }
}
