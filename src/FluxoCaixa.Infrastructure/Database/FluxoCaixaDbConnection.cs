using System.Data;
using Npgsql;

namespace FluxoCaixa.Infrastructure.Database;

public interface IFluxoCaixaDbConnection
{
    IDbConnection GetConnection();
}

public class FluxoCaixaDbConnection : IFluxoCaixaDbConnection
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;

    public FluxoCaixaDbConnection(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection GetConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }
}
