using System.Threading;
using System.Threading.Tasks;

namespace Events.Domain.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
