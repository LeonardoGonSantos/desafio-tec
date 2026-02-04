using System.Data;
using FluxoCaixa.Shared.Interfaces;
using Npgsql;

namespace FluxoCaixa.Infrastructure.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly IFluxoCaixaDbConnection _dbConnection;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    public UnitOfWork(IFluxoCaixaDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _connection = (NpgsqlConnection)_dbConnection.GetConnection();
        _transaction = _connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        _transaction?.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        _transaction?.Rollback();
        return Task.CompletedTask;
    }

    public IDbConnection GetConnection()
    {
        return _dbConnection.GetConnection();
    }

    public IDbTransaction? GetTransaction() => _transaction;

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }
}
