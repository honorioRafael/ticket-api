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

    public async Task<(IReadOnlyList<Venue> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Venues.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(v => v.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public void Update(Venue venue)
    {
        _context.Venues.Update(venue);
    }

    public void Remove(Venue venue)
    {
        _context.Venues.Remove(venue);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
