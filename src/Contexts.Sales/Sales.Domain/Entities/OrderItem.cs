namespace Sales.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid ticketTypeId, decimal unitPrice, int quantity)
    {
        if (unitPrice < 0)
            throw new ArgumentException("O preço unitário não pode ser negativo.", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantity));

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
