using System;
using System.Collections.Generic;

namespace Sales.Application.Commands;

public record CreateOrderCommand(Guid CustomerId, List<OrderItemInput> Items);
