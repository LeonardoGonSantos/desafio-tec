using FluxoCaixa.Domain.Consolidado.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Domain.Consolidado;

public class DataTests
{
    [Fact]
    public void Criar_ComDataValida_DeveRetornarSucesso()
    {
        var data = new DateTime(2025, 2, 3, 14, 30, 0);

        var result = Data.Criar(data);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Valor.Should().Be(new DateTime(2025, 2, 3));
    }

    [Fact]
    public void Criar_ComDataComHora_DeveNormalizarParaApenasData()
    {
        var dataComHora = new DateTime(2025, 1, 15, 23, 59, 59);

        var result = Data.Criar(dataComHora);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Valor.Should().Be(new DateTime(2025, 1, 15));
        result.Value.Valor.TimeOfDay.Should().Be(TimeSpan.Zero);
    }
}
