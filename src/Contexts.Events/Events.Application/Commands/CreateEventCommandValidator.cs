using FluentValidation;

namespace Events.Application.Commands;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.StartsAt).NotEmpty().WithMessage("A data de início é obrigatória.");
        RuleFor(x => x.EndsAt).NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThan(x => x.StartsAt).WithMessage("A data de fim deve ser posterior à data de início.");
        RuleFor(x => x.VenueId).NotEmpty().WithMessage("O ID do local é obrigatório.");
    }
}
