using TicketApi.Common.Exceptions;

namespace Sales.Domain.Entities;

public class OrderItem
{
    private readonly List<Ticket> _tickets = new();

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid ticketTypeId, decimal unitPrice, int quantity)
    {
        if (unitPrice < 0)
            throw new DomainException(DomainErrorCode.ValidationError, "O preço unitário não pode ser negativo.");
        if (quantity <= 0)
            throw new DomainException(DomainErrorCode.ValidationError, "A quantidade deve ser maior que zero.");

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
