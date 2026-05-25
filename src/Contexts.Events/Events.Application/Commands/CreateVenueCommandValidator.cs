using FluentValidation;

namespace Events.Application.Commands;

public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Address).NotEmpty().WithMessage("O endereço é obrigatório.");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("A capacidade deve ser maior que zero.");
    }
}
