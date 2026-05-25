using FluentValidation;

namespace Events.Application.Features.Organizers.RegisterOrganizer;

public class RegisterOrganizerCommandValidator : AbstractValidator<RegisterOrganizerCommand>
{
    public RegisterOrganizerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");
    }
}
