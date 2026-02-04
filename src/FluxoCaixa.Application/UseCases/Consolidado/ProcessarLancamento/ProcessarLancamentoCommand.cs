namespace FluxoCaixa.Application.UseCases.Consolidado.ProcessarLancamento;

public record ProcessarLancamentoCommand(
    Guid LancamentoId,
    string Tipo,
    decimal Valor,
    DateTime DataLancamento);
