namespace FluxoCaixa.Application.UseCases.Lancamentos.RegistrarLancamento;

public record RegistrarLancamentoCommand(
    string Tipo,
    decimal Valor,
    string Descricao,
    DateTime DataLancamento);
