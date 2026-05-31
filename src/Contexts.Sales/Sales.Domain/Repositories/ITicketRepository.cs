using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken = default);
    Task<Ticket?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    void Update(Ticket ticket);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
