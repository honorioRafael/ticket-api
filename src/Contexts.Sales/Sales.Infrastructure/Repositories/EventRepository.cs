using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;
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
