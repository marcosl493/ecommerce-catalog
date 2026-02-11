using FluentResults;

namespace Application.Reasons;

public sealed class ResourceNotFoundError(string resource) : Error($"{resource} nao foi encontrado(a)");

public sealed class BusinessLogicError(string message) : Error(message);

public class RequestValidationError : Error
{
    public RequestValidationError() : base("Requisicao invalida") { }

    public Dictionary<string, string[]> FieldReasonDictionary { get; init; } = default!;
}
