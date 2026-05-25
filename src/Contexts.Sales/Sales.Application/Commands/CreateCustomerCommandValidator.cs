using FluentValidation;

namespace Sales.Application.Commands;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
        RuleFor(x => x.Document).NotEmpty().WithMessage("O documento é obrigatório.");
    }
}
