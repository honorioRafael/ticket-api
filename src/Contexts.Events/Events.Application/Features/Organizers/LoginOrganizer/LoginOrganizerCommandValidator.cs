using FluentValidation;

namespace Events.Application.Features.Organizers.LoginOrganizer;

public class LoginOrganizerCommandValidator : AbstractValidator<LoginOrganizerCommand>
{
    public LoginOrganizerCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
