namespace FluxoCaixa.Application.UseCases.Lancamentos.ListarLancamentos;

public record ListarLancamentosQuery(
    DateTime? DataInicio,
    DateTime? DataFim,
    int Page,
    int PageSize);
