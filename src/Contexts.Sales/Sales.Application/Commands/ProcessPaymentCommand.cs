using System;

namespace Sales.Application.Commands;

public record ProcessPaymentCommand(Guid OrderId, string Method);
