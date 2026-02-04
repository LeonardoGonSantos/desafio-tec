namespace FluxoCaixa.Application.UseCases.Lancamentos.ListarLancamentos;

public record ListarLancamentosResponse(
    IReadOnlyList<LancamentoItemResponse> Lancamentos,
    int Page,
    int PageSize,
    int TotalCount);

public record LancamentoItemResponse(
    Guid Id,
    string Tipo,
    decimal Valor,
    string Descricao,
    DateTime DataLancamento,
    DateTime CreatedAt);
