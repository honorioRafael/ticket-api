using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly EventsDbContext _context;

    public VenueRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        await _context.Venues.AddAsync(venue, cancellationToken);
    }

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues.SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Venues.ToListAsync(cancellationToken);
    }

    public void Update(Venue venue)
    {
        _context.Venues.Update(venue);
    }
}

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

public class UnitOfWork : IUnitOfWork
{
    private readonly EventsDbContext _context;

    public UnitOfWork(EventsDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
