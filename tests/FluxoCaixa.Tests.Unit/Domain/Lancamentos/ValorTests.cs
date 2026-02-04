using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Domain.Lancamentos;

public class ValorTests
{
    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100.50)]
    [InlineData(999999999.99)]
    public void Criar_ComValorValido_DeveRetornarSucesso(decimal valor)
    {
        var result = Valor.Criar(valor);
        result.IsSuccess.Should().BeTrue();
        result.Value!.ValorDecimal.Should().Be(valor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void Criar_ComValorZeroOuNegativo_DeveRetornarFalha(decimal valor)
    {
        var result = Valor.Criar(valor);
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Valor deve ser maior que zero");
    }

    [Fact]
    public void Criar_ComValorAcimaDoLimite_DeveRetornarFalha()
    {
        var result = Valor.Criar(1000000000m);
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Valor excede o limite permitido");
    }
}
