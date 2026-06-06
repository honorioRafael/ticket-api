using FluentValidation;

namespace Sales.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do cliente é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
        RuleFor(x => x.Document).NotEmpty().WithMessage("O documento é obrigatório.");
    }
}
