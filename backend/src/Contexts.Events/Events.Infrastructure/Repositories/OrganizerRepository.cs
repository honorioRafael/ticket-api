using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Domain.ValueObjects;
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
        await _context.Organizers.AddAsync(organizer, cancellationToken);
    }

    public async Task<Organizer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Organizers.SingleOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailVo = new Email(email);
        return await _context.Organizers.SingleOrDefaultAsync(o => o.Email == emailVo, cancellationToken);
    }

    public void Update(Organizer organizer)
    {
        _context.Organizers.Update(organizer);
    }

    public void Remove(Organizer organizer)
    {
        _context.Organizers.Remove(organizer);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
