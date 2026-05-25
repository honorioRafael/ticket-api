using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class InvalidStateTransitionException : DomainException
{
    public override string Code => "INVALID_STATE_TRANSITION";

    public InvalidStateTransitionException(string message)
        : base(message)
    {
    }
}
