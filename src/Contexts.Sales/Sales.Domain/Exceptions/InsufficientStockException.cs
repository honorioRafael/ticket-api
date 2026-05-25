using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public override string Code => "INSUFFICIENT_STOCK";

    public InsufficientStockException(string message = "Não há estoque disponível suficiente de ingressos.") 
        : base(message)
    {
    }
}
