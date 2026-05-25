using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class InvalidPaymentStatusException : DomainException
{
    public override string Code => "INVALID_PAYMENT_STATUS";

    public InvalidPaymentStatusException(string message) 
        : base(message)
    {
    }
}
