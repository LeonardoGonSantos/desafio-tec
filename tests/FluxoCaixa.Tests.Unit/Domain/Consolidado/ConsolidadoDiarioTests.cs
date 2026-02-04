using FluxoCaixa.Domain.Consolidado.Entities;
using FluentAssertions;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Domain.Consolidado;

public class ConsolidadoDiarioTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarSucesso()
    {
        var data = DateTime.Today;

        var result = ConsolidadoDiario.Criar(data);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBeEmpty();
        result.Value.Data.Valor.Should().Be(data.Date);
        result.Value.Saldo.Valor.Should().Be(0);
        result.Value.TotalCreditos.Should().Be(0);
        result.Value.TotalDebitos.Should().Be(0);
        result.Value.QuantidadeLancamentos.Should().Be(0);
    }

    [Fact]
    public void Criar_ComSaldoInicial_DeveRetornarConsolidadoComSaldo()
    {
        var data = DateTime.Today;
        var saldoInicial = 100.50m;

        var result = ConsolidadoDiario.Criar(data, saldoInicial);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Saldo.Valor.Should().Be(saldoInicial);
    }

    [Fact]
    public void AdicionarCredito_ComValorValido_DeveAumentarSaldoETotalCreditos()
    {
        var consolidado = ConsolidadoDiario.Criar(DateTime.Today, 0).Value!;
        var valorCredito = 150.75m;

        var result = consolidado.AdicionarCredito(valorCredito);

        result.IsSuccess.Should().BeTrue();
        consolidado.Saldo.Valor.Should().Be(150.75m);
        consolidado.TotalCreditos.Should().Be(150.75m);
        consolidado.QuantidadeLancamentos.Should().Be(1);
    }

    [Fact]
    public void AdicionarDebito_ComValorValido_DeveDiminuirSaldoEAumentarTotalDebitos()
    {
        var consolidado = ConsolidadoDiario.Criar(DateTime.Today, 200m).Value!;
        var valorDebito = 50.25m;

        var result = consolidado.AdicionarDebito(valorDebito);

        result.IsSuccess.Should().BeTrue();
        consolidado.Saldo.Valor.Should().Be(149.75m);
        consolidado.TotalDebitos.Should().Be(50.25m);
        consolidado.QuantidadeLancamentos.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void AdicionarCredito_ComValorZeroOuNegativo_DeveRetornarFalha(decimal valor)
    {
        var consolidado = ConsolidadoDiario.Criar(DateTime.Today, 0).Value!;

        var result = consolidado.AdicionarCredito(valor);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Valor do crédito deve ser maior que zero");
        consolidado.QuantidadeLancamentos.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void AdicionarDebito_ComValorZeroOuNegativo_DeveRetornarFalha(decimal valor)
    {
        var consolidado = ConsolidadoDiario.Criar(DateTime.Today, 100m).Value!;

        var result = consolidado.AdicionarDebito(valor);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Valor do débito deve ser maior que zero");
        consolidado.Saldo.Valor.Should().Be(100m);
    }

    [Fact]
    public void AdicionarCreditoEMDebito_MultiplasVezes_QuantidadeLancamentosIncrementaCorretamente()
    {
        var consolidado = ConsolidadoDiario.Criar(DateTime.Today, 0).Value!;

        consolidado.AdicionarCredito(100m);
        consolidado.AdicionarCredito(50m);
        consolidado.AdicionarDebito(30m);

        consolidado.QuantidadeLancamentos.Should().Be(3);
        consolidado.TotalCreditos.Should().Be(150m);
        consolidado.TotalDebitos.Should().Be(30m);
        consolidado.Saldo.Valor.Should().Be(120m);
    }
}
