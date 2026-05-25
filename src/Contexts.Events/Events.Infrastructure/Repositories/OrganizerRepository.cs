using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories;

public class OrganizerRepository : IOrganizerRepository
{
    private readonly EventsDbContext _context;

    public OrganizerRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Organizer organizer, CancellationToken cancellationToken = default)
    {
        await _context.Set<Organizer>().AddAsync(organizer, cancellationToken);
    }

    public async Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Organizer>().SingleOrDefaultAsync(o => o.Email == email, cancellationToken);
    }
}
