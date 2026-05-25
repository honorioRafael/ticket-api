using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Reservation> reservations, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetActiveReservationsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reservation>> GetExpiredActiveReservationsAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Reservation reservation);
}
