using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.EditProduct;

public sealed record EditProductCommand(Guid Id, string? Name, string? Description, decimal? Price, bool? Active, ProductCategory? Category)
    : IRequest<Result<Product>>;
