using Events.Domain.Enums;
using TicketApi.Common.Exceptions;

namespace Events.Domain.Entities;

public class TicketType
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int TotalQuantity { get; private set; }
    public int AvailableQuantity { get; private set; }

    private TicketType() { }

    public TicketType(Guid eventId, string name, decimal price, int totalQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do tipo de ingresso não pode ser vazio.");
        if (price < 0)
            throw new DomainException("INVALID_PRICE", "O preço não pode ser negativo.");
        if (totalQuantity <= 0)
            throw new DomainException("INVALID_QUANTITY", "A quantidade total deve ser maior que zero.");

        Id = Guid.CreateVersion7();
        EventId = eventId;
        Name = name;
        Price = price;
        TotalQuantity = totalQuantity;
        AvailableQuantity = totalQuantity;
    }

    public void Update(string name, decimal price, int totalQuantity, EventStatus eventStatus)
    {
        if (eventStatus != EventStatus.Draft)
            throw new DomainException("TICKET_TYPE_READ_ONLY", "Não é possível modificar tipos de ingresso de um evento que não esteja em rascunho.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "O nome do tipo de ingresso não pode ser vazio.");
        if (price < 0)
            throw new DomainException("INVALID_PRICE", "O preço não pode ser negativo.");
        if (totalQuantity <= 0)
            throw new DomainException("INVALID_QUANTITY", "A quantidade total deve ser maior que zero.");

        int soldQuantity = TotalQuantity - AvailableQuantity;
        if (totalQuantity < soldQuantity)
            throw new DomainException("INVALID_TOTAL_QUANTITY", "A nova quantidade total não pode ser menor que os ingressos já vendidos.");

        Name = name;
        Price = price;
        TotalQuantity = totalQuantity;
        AvailableQuantity = totalQuantity - soldQuantity;
    }

    public void DecrementAvailableQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("INVALID_QUANTITY", "A quantidade a decrementar deve ser maior que zero.");
        if (AvailableQuantity < quantity)
            throw new DomainException("INSUFFICIENT_TICKETS", "Ingressos disponíveis insuficientes.");

        AvailableQuantity -= quantity;
    }

    public void IncrementAvailableQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("INVALID_QUANTITY", "A quantidade a incrementar deve ser maior que zero.");
        if (AvailableQuantity + quantity > TotalQuantity)
            throw new DomainException("EXCEEDS_TOTAL_QUANTITY", "Não é possível exceder a quantidade total de ingressos.");

        AvailableQuantity += quantity;
    }
}
