using FluentValidation;

namespace Events.Application.Features.Venues.UpdateVenue;

public class UpdateVenueCommandValidator : AbstractValidator<UpdateVenueCommand>
{
    public UpdateVenueCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do local é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("O endereço é obrigatório.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("A capacidade deve ser maior que zero.");
    }
}
