using FluentValidation;

namespace Sales.Application.Features.Payments.ProcessPayment;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("O ID do pedido é obrigatório.");
        RuleFor(x => x.Method).NotEmpty().WithMessage("O método de pagamento é obrigatório.")
            .Must(m => m.Equals("credit_card", StringComparison.OrdinalIgnoreCase) ||
                       m.Equals("pix", StringComparison.OrdinalIgnoreCase) ||
                       m.Equals("boleto", StringComparison.OrdinalIgnoreCase))
            .WithMessage("O método de pagamento deve ser credit_card, pix ou boleto.");
    }
}
