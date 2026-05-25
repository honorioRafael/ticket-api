using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IVenueRepository
{
    Task AddAsync(Venue venue, CancellationToken cancellationToken = default);
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venue>> GetAllAsync(CancellationToken cancellationToken = default);
    void Update(Venue venue);
}

public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetAllAsync(CancellationToken cancellationToken = default);
    void Update(Event @event);
}
