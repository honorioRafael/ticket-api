using System;
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

public class CapacityExceededException : DomainException
{
    public override string Code => "CAPACITY_EXCEEDED";

    public CapacityExceededException(string message = "A soma da quantidade de ingressos excede a capacidade do local.") 
        : base(message)
    {
    }
}

public class TicketTypeReadOnlyException : DomainException
{
    public override string Code => "TICKET_TYPE_READ_ONLY";

    public TicketTypeReadOnlyException(string message = "Tipos de ingresso só podem ser modificados com o evento em rascunho.") 
        : base(message)
    {
    }
}

public class InvalidStateTransitionException : DomainException
{
    public override string Code => "INVALID_STATE_TRANSITION";

    public InvalidStateTransitionException(string message) 
        : base(message)
    {
    }
}

public class EventNotPublishedException : DomainException
{
    public override string Code => "EVENT_NOT_PUBLISHED";

    public EventNotPublishedException(string message = "O evento não está publicado.") 
        : base(message)
    {
    }
}

public class VenueNotFoundException : DomainException
{
    public override string Code => "VENUE_NOT_FOUND";

    public VenueNotFoundException(string message = "Local não encontrado.") 
        : base(message)
    {
    }
}

public class EventNotFoundException : DomainException
{
    public override string Code => "EVENT_NOT_FOUND";

    public EventNotFoundException(string message = "Evento não encontrado.") 
        : base(message)
    {
    }
}
