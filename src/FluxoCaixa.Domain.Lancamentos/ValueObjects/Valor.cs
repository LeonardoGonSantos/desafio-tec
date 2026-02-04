using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Domain.Lancamentos.ValueObjects;

public class Valor : IEquatable<Valor>
{
    public decimal ValorDecimal { get; private set; }

    private Valor() { }

    private Valor(decimal valor)
    {
        ValorDecimal = valor;
    }

    public static Result<Valor> Criar(decimal valor)
    {
        if (valor <= 0)
            return Result<Valor>.Failure("Valor deve ser maior que zero");

        if (valor > 999999999.99m)
            return Result<Valor>.Failure("Valor excede o limite permitido");

        return Result<Valor>.Success(new Valor(valor));
    }

    public bool Equals(Valor? other) => other is not null && ValorDecimal == other.ValorDecimal;

    public override bool Equals(object? obj) => Equals(obj as Valor);

    public override int GetHashCode() => ValorDecimal.GetHashCode();

    public static implicit operator decimal(Valor valor) => valor.ValorDecimal;
}
