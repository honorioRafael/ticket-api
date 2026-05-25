namespace Sales.Domain.Enums;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public enum ReservationStatus
{
    Active,
    Confirmed,
    Expired,
    Cancelled
}

public enum TicketStatus
{
    Active,
    Used,
    Cancelled
}

public enum PaymentMethod
{
    CreditCard,
    Pix,
    Boleto
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded
}
