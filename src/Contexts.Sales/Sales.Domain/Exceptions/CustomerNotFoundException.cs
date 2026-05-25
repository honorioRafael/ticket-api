using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class CustomerNotFoundException : DomainException
{
    public override string Code => "CUSTOMER_NOT_FOUND";

    public CustomerNotFoundException(string message = "Cliente não encontrado.")
        : base(message)
    {
    }
}
