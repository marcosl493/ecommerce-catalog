using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviours;

public class ExceptionHandlingPipelineBehaviour<TRequest, TResponse>(ILogger<ExceptionHandlingPipelineBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase<TResponse>, new()
{
    private readonly ILogger<ExceptionHandlingPipelineBehaviour<TRequest, TResponse>> logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Untreated exception");
            return new TResponse().WithError("Erro desconhecido");
        }
    }
}