using Application.Reasons;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Application.Behaviours;

public class RequestValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase<TResponse>, new()
{
    private readonly IEnumerable<IValidator<TRequest>> validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var validationContext = new ValidationContext<TRequest>(request);

        var validatinResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

        var fieldReasonDictionary = validatinResults
                                    .SelectMany(result => result.Errors)
                                    .GroupBy(error => error.PropertyName)
                                    .ToDictionary(
                                        errorGroup => errorGroup.Key,
                                        errorGroup => errorGroup.Select(e => e.ErrorMessage).ToArray()
                                    );

        if (fieldReasonDictionary.Count != 0)
            return new TResponse().WithError(new RequestValidationError { FieldReasonDictionary = fieldReasonDictionary });

        return await next(cancellationToken);
    }
}
