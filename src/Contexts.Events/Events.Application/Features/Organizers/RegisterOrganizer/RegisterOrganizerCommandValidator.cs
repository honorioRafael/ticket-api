using FluentValidation;

namespace Events.Application.Features.Organizers.RegisterOrganizer;

public class RegisterOrganizerCommandValidator : AbstractValidator<RegisterOrganizerCommand>
{
    public RegisterOrganizerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O email informado é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");
    }
}
