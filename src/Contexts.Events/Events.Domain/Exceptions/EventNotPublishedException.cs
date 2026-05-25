using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class EventNotPublishedException : DomainException
{
    public override string Code => "EVENT_NOT_PUBLISHED";

    public EventNotPublishedException(string message = "O evento não está publicado.") 
        : base(message)
    {
    }
}
