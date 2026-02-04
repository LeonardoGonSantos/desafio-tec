using FluxoCaixa.Application.Interfaces.Repositories;
using FluxoCaixa.Application.UseCases.Lancamentos.RegistrarLancamento;
using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Shared.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Application;

public class RegistrarLancamentoUseCaseTests
{
    private readonly Mock<ILancamentoRepository> _repositoryMock;
    private readonly Mock<IOutboxRepository> _outboxMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RegistrarLancamentoUseCase>> _loggerMock;
    private readonly RegistrarLancamentoUseCase _useCase;

    public RegistrarLancamentoUseCaseTests()
    {
        _repositoryMock = new Mock<ILancamentoRepository>();
        _outboxMock = new Mock<IOutboxRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RegistrarLancamentoUseCase>>();
        _useCase = new RegistrarLancamentoUseCase(
            _repositoryMock.Object,
            _outboxMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ComDadosValidos_DeveRegistrarComSucesso()
    {
        var command = new RegistrarLancamentoCommand("Credito", 100.50m, "Venda", DateTime.Today);

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(x => x.InsertAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _outboxMock.Setup(x => x.InsertAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _useCase.ExecuteAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().NotBeEmpty();
        _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()), Times.Once);
        _outboxMock.Verify(x => x.InsertAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("")]
    public async Task ExecuteAsync_ComTipoInvalido_DeveRetornarFalha(string tipo)
    {
        var command = new RegistrarLancamentoCommand(tipo, 100m, "Teste", DateTime.Today);
        var result = await _useCase.ExecuteAsync(command);
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Contains("Tipo de lançamento inválido"));
        _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Lancamento>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
