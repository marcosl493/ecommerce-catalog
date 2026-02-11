using Application.Interfaces;
using FluentResults;
using MediatR;
using Application.Reasons;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.UploadProductImage;

public sealed class UploadProductImageHandler(
    IProductRepository repository,
    IStorageService storage
) : IRequestHandler<UploadProductImageCommand, Result<Product>>
{
    private readonly IProductRepository _repository = repository;
    private readonly IStorageService _storage = storage;

    public async Task<Result<Product>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Fail(new ResourceNotFoundError("Product"));

        var key = $"{Guid.NewGuid()}:{Path.GetExtension(request.FileName)}";
        var bucket = "products";

        var uri = await _storage.UploadAsync(bucket, key, request.Content, cancellationToken: cancellationToken);

        product.ChangeImage(ProductImage.Create(uri.AbsoluteUri));

        await _repository.UpdateAsync(product, cancellationToken);

        return Result.Ok(product);
    }
}
