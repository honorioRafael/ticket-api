using FluentValidation;

namespace Events.Application.Commands;

public class CreateTicketTypeCommandValidator : AbstractValidator<CreateTicketTypeCommand>
{
    public CreateTicketTypeCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty().WithMessage("O ID do evento é obrigatório.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("O preço deve ser maior ou igual a zero.");
        RuleFor(x => x.TotalQuantity).GreaterThan(0).WithMessage("A quantidade total deve ser maior que zero.");
    }
}
