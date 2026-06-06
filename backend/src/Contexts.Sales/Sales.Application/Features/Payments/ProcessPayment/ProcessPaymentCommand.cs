namespace Sales.Application.Features.Payments.ProcessPayment;

public record ProcessPaymentCommand(Guid OrderId, Guid CustomerId, string Method);
