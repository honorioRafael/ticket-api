using System;
using System.Collections.Generic;
using FluentValidation;

namespace Sales.Application.Commands;

public record CreateCustomerCommand(string Name, string Email, string Document);

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("O nome é obrigatório.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Um e-mail válido é obrigatório.");
        RuleFor(x => x.Document).NotEmpty().WithMessage("O documento é obrigatório.");
    }
}

public record OrderItemInput(Guid TicketTypeId, int Quantity);

public record CreateOrderCommand(Guid CustomerId, List<OrderItemInput> Items);

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

public record ProcessPaymentCommand(Guid OrderId, string Method);

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

public record PaymentWebhookCommand(Guid OrderId, string Status, string Method);

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
