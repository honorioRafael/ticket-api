using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

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
