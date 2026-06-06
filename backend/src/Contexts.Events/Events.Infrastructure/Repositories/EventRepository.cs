using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventsDbContext _context;

    public EventRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
    }

    public async Task AddTicketTypeAsync(TicketType ticketType, CancellationToken cancellationToken = default)
    {
        await _context.TicketTypes.AddAsync(ticketType, cancellationToken);
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.TicketTypes)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(e => e.TicketTypes)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Event> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Events.AsNoTracking().Include(e => e.TicketTypes);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.Period.Start)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }

    public void Remove(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
