using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SalesDbContext _context;

    public OrderRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .SingleOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetExpiredPendingOrdersAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        // Pending orders which have active reservations that are expired
        var expiredOrderIds = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Active && r.ExpiresAt < now)
            .Select(r => r.OrderId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Status == OrderStatus.Pending && expiredOrderIds.Contains(o.Id))
            .ToListAsync(cancellationToken);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }
}
