using Sales.Domain.Entities;

namespace Sales.Domain.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(Payment payment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
