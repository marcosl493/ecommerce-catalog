using Application.Interfaces;
using Application.Reasons;
using FluentResults;
using MediatR;

namespace Application.UseCases.DeleteProduct;

public sealed class DeleteProductHandler
    (
        IProductRepository repository
    ) : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository = repository;

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Fail(new ResourceNotFoundError("Produto"));

        await _repository.DeleteAsync(product, cancellationToken);
        return Result.Ok();
    }
}
