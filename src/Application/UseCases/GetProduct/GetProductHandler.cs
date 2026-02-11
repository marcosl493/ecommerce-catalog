using Application.Common;
using Application.Interfaces;
using Application.Reasons;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.GetProduct;

public sealed class GetProductHandler
    (
        IProductRepository repository
    ) : IRequestHandler<GetProductQuery, Result<PagedResult<Product>>>
{
    private readonly IProductRepository _repository = repository;

    public async Task<Result<PagedResult<Product>>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
        {
            var product = await _repository.GetByIdAsync(request.Id.Value, cancellationToken);
            if (product is null)
                return Result.Fail(new ResourceNotFoundError("Product"));

            var single = new PagedResult<Product>
            {
                Items = [product],
                TotalCount = 1,
                Page = 1,
                PageSize = 1
            };

            return Result.Ok(single);
        }

        var items = await _repository
            .QueryAsync
            (
                request.MinPrice,
                request.MaxPrice,
                request.Active,
                request.Category,
                request.Page,
                request.PageSize,
                cancellationToken
            );

        return Result.Ok(items);
    }
}
