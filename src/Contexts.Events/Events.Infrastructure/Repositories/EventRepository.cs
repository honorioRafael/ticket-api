using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }
}
