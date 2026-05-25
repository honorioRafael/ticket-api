using SharedKernel.Exceptions;

namespace Sales.Domain.Exceptions;

public class ReservationExpiredException : DomainException
{
    public override string Code => "RESERVATION_EXPIRED";

    public ReservationExpiredException(string message = "A reserva expirou.") 
        : base(message)
    {
    }
}
