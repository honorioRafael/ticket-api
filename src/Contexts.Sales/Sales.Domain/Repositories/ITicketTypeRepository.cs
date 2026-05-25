using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(TicketType ticketType);
}
