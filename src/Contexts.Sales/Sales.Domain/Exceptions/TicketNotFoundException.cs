using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class TicketNotFoundException : DomainException
{
    public override string Code => "TICKET_NOT_FOUND";

    public TicketNotFoundException(string message = "Código do ingresso não encontrado.")
        : base(message)
    {
    }
}
