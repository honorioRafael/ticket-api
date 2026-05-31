using Events.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetPublishedEventsEndingBeforeAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Event @event);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
