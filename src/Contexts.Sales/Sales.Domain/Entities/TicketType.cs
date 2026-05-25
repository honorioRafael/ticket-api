using Sales.Domain.Exceptions;

namespace Sales.Domain.Entities;

public class TicketType
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    private TicketType() { }

    public void DecrementStock(int quantity)
    {
        if (AvailableQuantity < quantity)
            throw new InsufficientStockException();

        AvailableQuantity -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        if (AvailableQuantity + quantity > TotalQuantity)
            throw new InvalidOperationException("Não é possível exceder a quantidade total.");

        AvailableQuantity += quantity;
    }
}
