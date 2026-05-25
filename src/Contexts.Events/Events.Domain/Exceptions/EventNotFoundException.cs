using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class EventNotFoundException : DomainException
{
    public override string Code => "EVENT_NOT_FOUND";

    public EventNotFoundException(string message = "Evento não encontrado.") 
        : base(message)
    {
    }
}
