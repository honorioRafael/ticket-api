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

public class CustomerRepository : ICustomerRepository
{
    private readonly SalesDbContext _context;

    public CustomerRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Document == document, cancellationToken);
    }
}

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

public class ReservationRepository : IReservationRepository
{
    private readonly SalesDbContext _context;

    public ReservationRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Reservation> reservations, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddRangeAsync(reservations, cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetActiveReservationsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var list = await _context.Reservations
            .Where(r => r.OrderId == orderId && r.Status == ReservationStatus.Active)
            .ToListAsync(cancellationToken);
        return list.AsReadOnly();
    }

    public async Task<IReadOnlyList<Reservation>> GetExpiredActiveReservationsAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var list = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Active && r.ExpiresAt < now)
            .ToListAsync(cancellationToken);
        return list.AsReadOnly();
    }

    public void Update(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
    }
}

public class TicketRepository : ITicketRepository
{
    private readonly SalesDbContext _context;

    public TicketRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddAsync(ticket, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken = default)
    {
        await _context.Tickets.AddRangeAsync(tickets, cancellationToken);
    }

    public async Task<Ticket?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets.SingleOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
    }
}

public class PaymentRepository : IPaymentRepository
{
    private readonly SalesDbContext _context;

    public PaymentRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.SingleOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly SalesDbContext _context;

    public TicketTypeRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<TicketType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TicketTypes.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public void Update(TicketType ticketType)
    {
        _context.TicketTypes.Update(ticketType);
    }
}

public class EventRepository : IEventRepository
{
    private readonly SalesDbContext _context;

    public EventRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetPublishedEventsEndingBeforeAsync(DateTime now, CancellationToken cancellationToken = default)
    {
        var list = await _context.Events
            .Where(e => e.Status == "published" && e.EndsAt < now)
            .ToListAsync(cancellationToken);
        return list.AsReadOnly();
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly SalesDbContext _context;

    public UnitOfWork(SalesDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new Sales.Domain.Exceptions.InsufficientStockException("Concurrency conflict: ticket stock is no longer available.");
        }
    }
}
