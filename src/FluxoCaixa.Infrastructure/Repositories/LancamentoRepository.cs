using System.Data;
using Dapper;
using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluxoCaixa.Shared.Interfaces;

namespace FluxoCaixa.Infrastructure.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public LancamentoRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task InsertAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO lancamentos (id, tipo, valor, descricao, data_lancamento, created_at)
            VALUES (@Id, @Tipo, @Valor, @Descricao, @DataLancamento, @CreatedAt)";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                lancamento.Id,
                Tipo = (int)lancamento.Tipo,
                Valor = lancamento.Valor.ValorDecimal,
                lancamento.Descricao,
                lancamento.DataLancamento,
                lancamento.CreatedAt
            }, transaction: trans, cancellationToken: cancellationToken));
    }

    public async Task<Lancamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, tipo, valor, descricao, data_lancamento as DataLancamento, created_at as CreatedAt
            FROM lancamentos WHERE id = @Id";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        var dto = await conn.QueryFirstOrDefaultAsync<LancamentoDto>(
            new CommandDefinition(sql, new { Id = id }, transaction: trans, cancellationToken: cancellationToken));

        return dto?.ToEntity();
    }

    public async Task<IEnumerable<Lancamento>> ListAsync(
        DateTime? dataInicio,
        DateTime? dataFim,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            SELECT id, tipo, valor, descricao, data_lancamento as DataLancamento, created_at as CreatedAt
            FROM lancamentos WHERE 1=1";
        var parameters = new DynamicParameters();

        if (dataInicio.HasValue)
        {
            sql += " AND data_lancamento >= @DataInicio";
            parameters.Add("DataInicio", dataInicio.Value);
        }
        if (dataFim.HasValue)
        {
            sql += " AND data_lancamento <= @DataFim";
            parameters.Add("DataFim", dataFim.Value);
        }

        sql += " ORDER BY data_lancamento DESC, created_at DESC LIMIT @PageSize OFFSET @Offset";
        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (page - 1) * pageSize);

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        var dtos = await conn.QueryAsync<LancamentoDto>(
            new CommandDefinition(sql, parameters, transaction: trans, cancellationToken: cancellationToken));

        return dtos.Select(d => d.ToEntity());
    }

    private class LancamentoDto
    {
        public Guid Id { get; set; }
        public int Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataLancamento { get; set; }
        public DateTime CreatedAt { get; set; }

        public Lancamento ToEntity()
        {
            return Lancamento.Reconstitute(Id, (TipoLancamento)Tipo, Valor, Descricao, DataLancamento, CreatedAt);
        }
    }
}
