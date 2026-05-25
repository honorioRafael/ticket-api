using System.Threading;
using System.Threading.Tasks;

namespace Sales.Domain.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
