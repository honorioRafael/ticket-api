using Microsoft.EntityFrameworkCore;
using Events.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
