using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class TicketAlreadyUsedException : DomainException
{
    public override string Code => "TICKET_ALREADY_USED";

    public TicketAlreadyUsedException(string message = "O ingresso já foi utilizado.") 
        : base(message)
    {
    }
}
