using Dapper;
using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Domain.Consolidado.Entities;
using FluxoCaixa.Shared.Interfaces;

namespace FluxoCaixa.Infrastructure.Repositories;

public class ConsolidadoRepository : IConsolidadoRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public ConsolidadoRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ConsolidadoDiario?> GetByDataAsync(DateTime data, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, data, saldo, total_creditos as TotalCreditos, total_debitos as TotalDebitos,
                   quantidade_lancamentos as QuantidadeLancamentos, updated_at as UpdatedAt, created_at as CreatedAt
            FROM consolidado_diario WHERE data = @Data";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        var dto = await conn.QueryFirstOrDefaultAsync<ConsolidadoDto>(
            new CommandDefinition(sql, new { Data = data.Date }, transaction: trans, cancellationToken: cancellationToken));

        return dto?.ToEntity();
    }

    public async Task InsertAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO consolidado_diario (id, data, saldo, total_creditos, total_debitos, quantidade_lancamentos, updated_at, created_at)
            VALUES (@Id, @Data, @Saldo, @TotalCreditos, @TotalDebitos, @QuantidadeLancamentos, @UpdatedAt, @CreatedAt)";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                consolidado.Id,
                Data = consolidado.Data.Valor,
                Saldo = consolidado.Saldo.Valor,
                consolidado.TotalCreditos,
                consolidado.TotalDebitos,
                consolidado.QuantidadeLancamentos,
                consolidado.UpdatedAt,
                consolidado.CreatedAt
            }, transaction: trans, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(ConsolidadoDiario consolidado, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE consolidado_diario
            SET saldo = @Saldo, total_creditos = @TotalCreditos, total_debitos = @TotalDebitos,
                quantidade_lancamentos = @QuantidadeLancamentos, updated_at = @UpdatedAt
            WHERE id = @Id";

        var conn = _unitOfWork.GetConnection();
        var trans = _unitOfWork.GetTransaction();

        await conn.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                consolidado.Id,
                Saldo = consolidado.Saldo.Valor,
                consolidado.TotalCreditos,
                consolidado.TotalDebitos,
                consolidado.QuantidadeLancamentos,
                consolidado.UpdatedAt
            }, transaction: trans, cancellationToken: cancellationToken));
    }

    private class ConsolidadoDto
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public decimal Saldo { get; set; }
        public decimal TotalCreditos { get; set; }
        public decimal TotalDebitos { get; set; }
        public int QuantidadeLancamentos { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ConsolidadoDiario ToEntity()
        {
            return ConsolidadoDiario.Reconstitute(Id, Data, Saldo, TotalCreditos, TotalDebitos, QuantidadeLancamentos, UpdatedAt, CreatedAt);
        }
    }
}
