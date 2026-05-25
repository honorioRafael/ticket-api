using FluentValidation;

namespace Events.Application.Features.Events.UpdateEvent;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do evento é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.StartsAt).NotEmpty().WithMessage("A data de início é obrigatória.");
        RuleFor(x => x.EndsAt).NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThan(x => x.StartsAt).WithMessage("A data de fim deve ser posterior à data de início.");
        RuleFor(x => x.VenueId).NotEmpty().WithMessage("O ID do local é obrigatório.");
    }
}
