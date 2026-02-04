using FluxoCaixa.Application.UseCases.Lancamentos.ListarLancamentos;
using FluxoCaixa.Application.UseCases.Lancamentos.ObterLancamento;
using FluxoCaixa.Application.UseCases.Lancamentos.RegistrarLancamento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxoCaixa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LancamentosController : ControllerBase
{
    private readonly IRegistrarLancamentoUseCase _registrarUseCase;
    private readonly IListarLancamentosUseCase _listarUseCase;
    private readonly IObterLancamentoUseCase _obterUseCase;
    private readonly ILogger<LancamentosController> _logger;

    public LancamentosController(
        IRegistrarLancamentoUseCase registrarUseCase,
        IListarLancamentosUseCase listarUseCase,
        IObterLancamentoUseCase obterUseCase,
        ILogger<LancamentosController> logger)
    {
        _registrarUseCase = registrarUseCase;
        _listarUseCase = listarUseCase;
        _obterUseCase = obterUseCase;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegistrarLancamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(
        [FromBody] RegistrarLancamentoCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _registrarUseCase.ExecuteAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Erro ao registrar lançamento",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join(", ", result.Errors)
            });
        }

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListarLancamentosResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new ListarLancamentosQuery(dataInicio, dataFim, page, pageSize);
        var result = await _listarUseCase.ExecuteAsync(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ObterLancamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new ObterLancamentoQuery(id);
        var result = await _obterUseCase.ExecuteAsync(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new ProblemDetails { Title = "Lançamento não encontrado", Status = 404, Detail = string.Join(", ", result.Errors) });

        return Ok(result.Value);
    }
}
