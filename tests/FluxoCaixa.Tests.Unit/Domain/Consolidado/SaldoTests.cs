using FluxoCaixa.Domain.Consolidado.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Domain.Consolidado;

public class SaldoTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(100.50)]
    [InlineData(-50.25)]
    public void Criar_ComQualquerValor_DeveRetornarSucesso(decimal valor)
    {
        var result = Saldo.Criar(valor);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Valor.Should().Be(valor);
    }
}
