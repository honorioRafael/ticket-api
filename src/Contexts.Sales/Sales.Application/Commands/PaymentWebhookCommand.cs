using System;

namespace Sales.Application.Commands;

public record PaymentWebhookCommand(Guid OrderId, string Status, string Method);
