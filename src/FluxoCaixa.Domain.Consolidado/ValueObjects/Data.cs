using FluxoCaixa.Shared.Results;

namespace FluxoCaixa.Domain.Consolidado.ValueObjects;

public class Data
{
    public DateTime Valor { get; private set; }

    private Data() { }

    private Data(DateTime valor)
    {
        Valor = valor.Date;
    }

    public static Result<Data> Criar(DateTime data)
    {
        return Result<Data>.Success(new Data(data));
    }
}
