using Microsoft.EntityFrameworkCore;
using Events.Domain.Entities;
using Sales.Domain.Repositories;
using Sales.Infrastructure.Contexts;

namespace Sales.Infrastructure.Repositories;

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

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
