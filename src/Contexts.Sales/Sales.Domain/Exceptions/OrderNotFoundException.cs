using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class OrderNotFoundException : DomainException
{
    public override string Code => "ORDER_NOT_FOUND";

    public OrderNotFoundException(string message = "Pedido não encontrado.")
        : base(message)
    {
    }
}
