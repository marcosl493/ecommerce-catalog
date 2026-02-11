using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.CreateProduct;

public sealed class CreateProductCommand
    : IRequest<Result<CreateProductResponse>>
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public decimal Price { get; init; }
    public ProductCategory Category { get; init; } = ProductCategory.Undefined;
}
