using FluxoCaixa.Application.UseCases.Consolidado.ObterConsolidadoDiario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsolidadoController : ControllerBase
{
    private readonly IObterConsolidadoDiarioUseCase _useCase;
    private readonly ILogger<ConsolidadoController> _logger;

    public ConsolidadoController(
        IObterConsolidadoDiarioUseCase useCase,
        ILogger<ConsolidadoController> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    [HttpGet("{data:datetime}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "data" })]
    [ProducesResponseType(typeof(ObterConsolidadoDiarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorData(
        DateTime data,
        CancellationToken cancellationToken)
    {
        var query = new ObterConsolidadoDiarioQuery(data.Date);
        var result = await _useCase.ExecuteAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Consolidado não encontrado",
                Status = StatusCodes.Status404NotFound,
                Detail = string.Join(", ", result.Errors)
            });
        }

        return Ok(result.Value);
    }
}
