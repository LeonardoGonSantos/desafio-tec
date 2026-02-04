using FluxoCaixa.Shared.Exceptions;

namespace FluxoCaixa.Domain.Lancamentos.Exceptions;

public class LancamentoDomainException : DomainException
{
    public LancamentoDomainException(string message) : base(message) { }

    public LancamentoDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
