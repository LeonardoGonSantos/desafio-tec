namespace FluxoCaixa.Application.UseCases.Lancamentos.ObterLancamento;

public record ObterLancamentoResponse(
    Guid Id,
    string Tipo,
    decimal Valor,
    string Descricao,
    DateTime DataLancamento,
    DateTime CreatedAt);
