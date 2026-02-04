using FluxoCaixa.Domain.Consolidado.ValueObjects;
using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Domain.Consolidado.Entities;

public class ConsolidadoDiario
{
    public Guid Id { get; private set; }
    public Data Data { get; private set; } = null!;
    public Saldo Saldo { get; private set; } = null!;
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public int QuantidadeLancamentos { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ConsolidadoDiario() { }

    public static Result<ConsolidadoDiario> Criar(DateTime data, decimal saldoInicial = 0)
    {
        var dataResult = Data.Criar(data);
        if (dataResult.IsFailure)
            return Result<ConsolidadoDiario>.Failure(dataResult.Errors);

        var saldoResult = Saldo.Criar(saldoInicial);
        if (saldoResult.IsFailure)
            return Result<ConsolidadoDiario>.Failure(saldoResult.Errors);

        var consolidado = new ConsolidadoDiario
        {
            Id = Guid.NewGuid(),
            Data = dataResult.Value!,
            Saldo = saldoResult.Value!,
            TotalCreditos = 0,
            TotalDebitos = 0,
            QuantidadeLancamentos = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result<ConsolidadoDiario>.Success(consolidado);
    }

    public Result AdicionarCredito(decimal valor)
    {
        if (valor <= 0)
            return Result.Failure("Valor do crédito deve ser maior que zero");

        TotalCreditos += valor;
        QuantidadeLancamentos++;

        var novoSaldo = Saldo.Valor + valor;
        var saldoResult = Saldo.Criar(novoSaldo);

        if (saldoResult.IsFailure)
            return Result.Failure(saldoResult.Errors);

        Saldo = saldoResult.Value!;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AdicionarDebito(decimal valor)
    {
        if (valor <= 0)
            return Result.Failure("Valor do débito deve ser maior que zero");

        TotalDebitos += valor;
        QuantidadeLancamentos++;

        var novoSaldo = Saldo.Valor - valor;
        var saldoResult = Saldo.Criar(novoSaldo);

        if (saldoResult.IsFailure)
            return Result.Failure(saldoResult.Errors);

        Saldo = saldoResult.Value!;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public static ConsolidadoDiario Reconstitute(
        Guid id,
        DateTime data,
        decimal saldo,
        decimal totalCreditos,
        decimal totalDebitos,
        int quantidadeLancamentos,
        DateTime updatedAt,
        DateTime createdAt)
    {
        var dataResult = Data.Criar(data);
        var saldoResult = Saldo.Criar(saldo);
        return new ConsolidadoDiario
        {
            Id = id,
            Data = dataResult.Value!,
            Saldo = saldoResult.Value!,
            TotalCreditos = totalCreditos,
            TotalDebitos = totalDebitos,
            QuantidadeLancamentos = quantidadeLancamentos,
            UpdatedAt = updatedAt,
            CreatedAt = createdAt
        };
    }
}
