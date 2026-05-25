using Sales.Domain.Enums;

namespace Sales.Domain.Entities;

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public ReservationStatus Status { get; private set; }

    private Reservation() { }

    public Reservation(Guid orderId, Guid ticketTypeId, int quantity, DateTime placedAt)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantity));

        Id = Guid.CreateVersion7();
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        Quantity = quantity;
        ExpiresAt = placedAt.AddMinutes(15);
        Status = ReservationStatus.Active;
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Não é possível confirmar a reserva a partir do status {Status}.");

        Status = ReservationStatus.Confirmed;
    }

    public void Expire()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Não é possível expirar a reserva a partir do status {Status}.");

        Status = ReservationStatus.Expired;
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException($"Não é possível cancelar a reserva a partir do status {Status}.");

        Status = ReservationStatus.Cancelled;
    }
}
