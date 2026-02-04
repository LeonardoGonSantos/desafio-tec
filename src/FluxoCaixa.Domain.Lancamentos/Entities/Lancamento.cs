using FluxoCaixa.Domain.Lancamentos.Events;
using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Domain.Lancamentos.Entities;

public class Lancamento
{
    public Guid Id { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public Valor Valor { get; private set; } = null!;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataLancamento { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Lancamento() { }

    public static Result<Lancamento> Criar(
        TipoLancamento tipo,
        decimal valorDecimal,
        string descricao,
        DateTime dataLancamento)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(descricao))
            erros.Add("Descrição é obrigatória");

        if (descricao?.Length > 200)
            erros.Add("Descrição não pode ter mais de 200 caracteres");

        var valorResult = Valor.Criar(valorDecimal);
        if (valorResult.IsFailure)
            erros.AddRange(valorResult.Errors);

        if (erros.Any())
            return Result<Lancamento>.Failure(erros);

        var valor = valorResult.Value!;
        var entidade = new Lancamento
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            Valor = valor,
            Descricao = descricao.Trim(),
            DataLancamento = dataLancamento.Date,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Lancamento>.Success(entidade);
    }

    public LancamentoRegistradoEvent GerarEvento()
    {
        return new LancamentoRegistradoEvent(
            Id,
            Tipo.ToString(),
            Valor.ValorDecimal,
            DataLancamento);
    }

    public static Lancamento Reconstitute(
        Guid id,
        TipoLancamento tipo,
        decimal valorDecimal,
        string descricao,
        DateTime dataLancamento,
        DateTime createdAt)
    {
        var valorResult = Valor.Criar(valorDecimal);
        var valor = valorResult.IsSuccess ? valorResult.Value! : throw new InvalidOperationException(valorResult.Errors.FirstOrDefault());
        return new Lancamento
        {
            Id = id,
            Tipo = tipo,
            Valor = valor,
            Descricao = descricao,
            DataLancamento = dataLancamento,
            CreatedAt = createdAt
        };
    }
}
