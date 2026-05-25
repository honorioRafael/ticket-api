using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class CapacityExceededException : DomainException
{
    public override string Code => "CAPACITY_EXCEEDED";

    public CapacityExceededException(string message = "A soma da quantidade de ingressos excede a capacidade do local.") 
        : base(message)
    {
    }
}
