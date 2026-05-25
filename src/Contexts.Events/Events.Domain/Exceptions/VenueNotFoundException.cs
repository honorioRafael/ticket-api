using SharedKernel.Exceptions;

namespace Events.Domain.Exceptions;

public class VenueNotFoundException : DomainException
{
    public override string Code => "VENUE_NOT_FOUND";

    public VenueNotFoundException(string message = "Local não encontrado.")
        : base(message)
    {
    }
}
