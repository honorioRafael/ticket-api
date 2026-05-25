using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IOrganizerRepository
{
    Task AddAsync(Organizer organizer, CancellationToken cancellationToken = default);
    Task<Organizer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
