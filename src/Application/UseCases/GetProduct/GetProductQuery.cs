using MediatR;
using Domain.Entities;
using Application.Common;
using FluentResults;

namespace Application.UseCases.GetProduct;

public sealed class GetProductQuery : IRequest<Result<PagedResult<Product>>>
{
    public Guid? Id { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool? Active { get; init; }
    public ProductCategory? Category { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
