using FluentValidation;

namespace Events.Application.Features.Organizers.LoginOrganizer;

public class LoginOrganizerCommandValidator : AbstractValidator<LoginOrganizerCommand>
{
    public LoginOrganizerCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O email informado é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
