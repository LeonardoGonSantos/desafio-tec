using FluxoCaixa.Shared.Events;

namespace FluxoCaixa.Domain.Lancamentos.Events;

public record LancamentoRegistradoEvent : IDomainEvent
{
    public Guid LancamentoId { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public decimal Valor { get; init; }
    public DateTime DataLancamento { get; init; }
    public DateTime OccurredAt { get; init; }

    public LancamentoRegistradoEvent(
        Guid lancamentoId,
        string tipo,
        decimal valor,
        DateTime dataLancamento)
    {
        LancamentoId = lancamentoId;
        Tipo = tipo;
        Valor = valor;
        DataLancamento = dataLancamento;
        OccurredAt = DateTime.UtcNow;
    }
}
