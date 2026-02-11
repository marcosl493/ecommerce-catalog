using Application.Interfaces;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.CreateProduct;

public sealed class CreateProductHandler
    (
        IProductRepository repository
    )
        : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _repository = repository;

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Category);

        await _repository.AddAsync(product, cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}
