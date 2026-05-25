using System;
using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public override string Code => "INSUFFICIENT_STOCK";

    public InsufficientStockException(string message = "Não há estoque disponível suficiente de ingressos.") 
        : base(message)
    {
    }
}

public class EventNotActiveException : DomainException
{
    public override string Code => "EVENT_NOT_ACTIVE";

    public EventNotActiveException(string message = "O evento não está ativo ou já começou.") 
        : base(message)
    {
    }
}

public class OrderNotFoundException : DomainException
{
    public override string Code => "ORDER_NOT_FOUND";

    public OrderNotFoundException(string message = "Pedido não encontrado.") 
        : base(message)
    {
    }
}

public class CustomerNotFoundException : DomainException
{
    public override string Code => "CUSTOMER_NOT_FOUND";

    public CustomerNotFoundException(string message = "Cliente não encontrado.") 
        : base(message)
    {
    }
}

public class ReservationExpiredException : DomainException
{
    public override string Code => "RESERVATION_EXPIRED";

    public ReservationExpiredException(string message = "A reserva expirou.") 
        : base(message)
    {
    }
}

public class InvalidPaymentStatusException : DomainException
{
    public override string Code => "INVALID_PAYMENT_STATUS";

    public InvalidPaymentStatusException(string message) 
        : base(message)
    {
    }
}

public class TicketNotFoundException : DomainException
{
    public override string Code => "TICKET_NOT_FOUND";

    public TicketNotFoundException(string message = "Código do ingresso não encontrado.") 
        : base(message)
    {
    }
}

public class TicketAlreadyUsedException : DomainException
{
    public override string Code => "TICKET_ALREADY_USED";

    public TicketAlreadyUsedException(string message = "O ingresso já foi utilizado.") 
        : base(message)
    {
    }
}

public class TicketCancelledException : DomainException
{
    public override string Code => "TICKET_CANCELLED";

    public TicketCancelledException(string message = "O ingresso foi cancelado.") 
        : base(message)
    {
    }
}
