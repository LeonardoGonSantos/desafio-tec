using FluxoCaixa.Shared.Exceptions;

namespace FluxoCaixa.Domain.Consolidado.Exceptions;

public class ConsolidadoDomainException : DomainException
{
    public ConsolidadoDomainException(string message) : base(message) { }

    public ConsolidadoDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
