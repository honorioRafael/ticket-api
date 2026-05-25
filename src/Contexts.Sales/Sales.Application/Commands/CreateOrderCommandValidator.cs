using FluentValidation;

namespace Sales.Application.Commands;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("O ID do cliente é obrigatório.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.TicketTypeId).NotEmpty().WithMessage("O ID do tipo de ingresso é obrigatório.");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        });
    }
}
