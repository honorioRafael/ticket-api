using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetExpiredPendingOrdersAsync(DateTime now, CancellationToken cancellationToken = default);
    void Update(Order order);
}
