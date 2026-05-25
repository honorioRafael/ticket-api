using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
}

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetExpiredPendingOrdersAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Order order);
}

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Reservation> reservations, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetActiveReservationsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetExpiredActiveReservationsAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Reservation reservation);
}

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken = default);
    Task<Ticket?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    void Update(Ticket ticket);
}

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Payment payment);
}

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(TicketType ticketType);
}

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetPublishedEventsEndingBeforeAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Event @event);
}

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
