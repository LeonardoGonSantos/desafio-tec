using System.Data;

namespace FluxoCaixa.Shared.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    IDbConnection GetConnection();
    IDbTransaction? GetTransaction();
}
