using FluxoCaixa.Domain.Lancamentos.Entities;
using FluxoCaixa.Domain.Lancamentos.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FluxoCaixa.Tests.Unit.Domain.Lancamentos;

public class LancamentoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarSucesso()
    {
        var tipo = TipoLancamento.Credito;
        var valor = 100.50m;
        var descricao = "Venda de produto";
        var dataLancamento = DateTime.Today;

        var result = Lancamento.Criar(tipo, valor, descricao, dataLancamento);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBeEmpty();
        result.Value.Tipo.Should().Be(tipo);
        result.Value.Valor.ValorDecimal.Should().Be(valor);
        result.Value.Descricao.Should().Be(descricao);
        result.Value.DataLancamento.Should().Be(dataLancamento.Date);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComDescricaoInvalida_DeveRetornarFalha(string? descricaoInvalida)
    {
        var result = Lancamento.Criar(TipoLancamento.Debito, 50m, descricaoInvalida!, DateTime.Today);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Descrição é obrigatória");
    }

    [Fact]
    public void Criar_ComDescricaoMuitoLonga_DeveRetornarFalha()
    {
        var descricao = new string('A', 201);
        var result = Lancamento.Criar(TipoLancamento.Credito, 100m, descricao, DateTime.Today);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Descrição não pode ter mais de 200 caracteres");
    }

    [Fact]
    public void GerarEvento_DeveRetornarEventoComDadosCorretos()
    {
        var lancamento = Lancamento.Criar(TipoLancamento.Credito, 150.75m, "Pagamento", DateTime.Today).Value!;
        var evento = lancamento.GerarEvento();

        evento.Should().NotBeNull();
        evento.LancamentoId.Should().Be(lancamento.Id);
        evento.Tipo.Should().Be("Credito");
        evento.Valor.Should().Be(150.75m);
        evento.DataLancamento.Should().Be(DateTime.Today);
    }
}
