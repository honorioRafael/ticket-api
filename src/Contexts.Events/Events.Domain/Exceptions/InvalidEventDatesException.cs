using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class InvalidEventDatesException : DomainException
{
    public override string Code => "INVALID_EVENT_DATES";

    public InvalidEventDatesException(string message = "A data de fim do evento deve ser posterior à data de início.")
        : base(message)
    {
    }
}
