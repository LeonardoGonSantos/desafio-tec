using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Domain.Consolidado.ValueObjects;

public class Saldo
{
    public decimal Valor { get; private set; }

    private Saldo() { }

    private Saldo(decimal valor)
    {
        Valor = valor;
    }

    public static Result<Saldo> Criar(decimal valor)
    {
        return Result<Saldo>.Success(new Saldo(valor));
    }
}
