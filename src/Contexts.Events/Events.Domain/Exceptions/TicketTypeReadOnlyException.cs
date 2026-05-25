using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class TicketTypeReadOnlyException : DomainException
{
    public override string Code => "TICKET_TYPE_READ_ONLY";

    public TicketTypeReadOnlyException(string message = "Tipos de ingresso só podem ser modificados com o evento em rascunho.") 
        : base(message)
    {
    }
}
