using TicketApi.Common.Exceptions;

namespace Events.Domain.ValueObjects;

public record DateTimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateTimeRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new DomainException(DomainErrorCode.RuleViolation, "A data de término do evento deve ser posterior à data de início.");

        Start = start;
        End = end;
    }

    public bool Contains(DateTime now)
    {
        return now > Start && now < End;
    }
}
