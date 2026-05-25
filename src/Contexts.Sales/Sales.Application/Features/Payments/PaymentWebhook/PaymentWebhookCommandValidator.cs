using FluentValidation;

namespace Sales.Application.Features.Payments.PaymentWebhook;

public class PaymentWebhookCommandValidator : AbstractValidator<PaymentWebhookCommand>
{
    public PaymentWebhookCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("O ID do pedido é obrigatório.");
        RuleFor(x => x.Status).NotEmpty().WithMessage("O status é obrigatório.")
            .Must(s => s.Equals("paid", StringComparison.OrdinalIgnoreCase) ||
                       s.Equals("failed", StringComparison.OrdinalIgnoreCase))
            .WithMessage("O status deve ser paid ou failed.");
        RuleFor(x => x.Method).NotEmpty().WithMessage("O método de pagamento é obrigatório.");
    }
}
