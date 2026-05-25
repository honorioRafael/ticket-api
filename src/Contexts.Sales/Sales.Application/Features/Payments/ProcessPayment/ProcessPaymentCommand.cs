namespace Sales.Application.Features.Payments.ProcessPayment;

public record ProcessPaymentCommand(Guid OrderId, string Method);
