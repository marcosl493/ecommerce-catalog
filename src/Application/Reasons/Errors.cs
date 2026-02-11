using FluentResults;

namespace Application.Reasons;

public sealed class ResourceNotFoundError(string resource) : Error($"{resource} não foi encontrado(a)");

public sealed class BusinessLogicError(string message) : Error(message);

public class RequestValidationError : Error
{
    public RequestValidationError() : base("Requisição inválida") { }

    public Dictionary<string, string[]> FieldReasonDictionary { get; init; } = default!;
}
