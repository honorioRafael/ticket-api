using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IOrganizerRepository
{
    Task AddAsync(Organizer organizer, CancellationToken cancellationToken = default);
    Task<Organizer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Update(Organizer organizer);
    void Remove(Organizer organizer);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
