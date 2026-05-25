using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class EventNotActiveException : DomainException
{
    public override string Code => "EVENT_NOT_ACTIVE";

    public EventNotActiveException(string message = "O evento não está ativo ou já começou.")
        : base(message)
    {
    }
}
