using System;
using System.Collections.Generic;
using System.Linq;
using Sales.Domain.Enums;

namespace Sales.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _orderItems = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    public Order(Guid customerId, IEnumerable<(Guid TicketTypeId, decimal UnitPrice, int Quantity)> items)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("O pedido deve conter pelo menos um item.", nameof(items));

        Id = Guid.CreateVersion7();
        CustomerId = customerId;
        PlacedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;

        foreach (var item in items)
        {
            var orderItem = new OrderItem(Id, item.TicketTypeId, item.UnitPrice, item.Quantity);
            _orderItems.Add(orderItem);
        }

        TotalAmount = _orderItems.Sum(i => i.UnitPrice * i.Quantity);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Não é possível confirmar o pedido a partir do status {Status}.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Não é possível cancelar o pedido a partir do status {Status}.");

        Status = OrderStatus.Cancelled;
    }
}
