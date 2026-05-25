using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class TicketCancelledException : DomainException
{
    public override string Code => "TICKET_CANCELLED";

    public TicketCancelledException(string message = "O ingresso foi cancelado.") 
        : base(message)
    {
    }
}
