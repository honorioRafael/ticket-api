using FluentValidation;

namespace Sales.Application.Features.Customers.LoginCustomer;

public class LoginCustomerCommandValidator : AbstractValidator<LoginCustomerCommand>
{
    public LoginCustomerCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
