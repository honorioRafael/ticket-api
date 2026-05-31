using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IVenueRepository
{
    Task AddAsync(Venue venue, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Venue> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    void Update(Venue venue);
    void Remove(Venue venue);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
