namespace FluxoCaixa.Application.UseCases.Consolidado.ObterConsolidadoDiario;

public record ObterConsolidadoDiarioResponse(
    DateTime Data,
    decimal Saldo,
    decimal TotalCreditos,
    decimal TotalDebitos,
    int QuantidadeLancamentos);
