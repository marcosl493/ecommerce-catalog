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

    private const int MaxBytes = 10 * 1024 * 1024; // 10 MB

    public async Task<Result<Product>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Fail(new ResourceNotFoundError("Product"));

        if (request.Content is null || request.Content.Length == 0)
            return Result.Fail(new BusinessLogicError("Arquivo inválido ou vazio."));

        if (request.Content.Length > MaxBytes)
            return Result.Fail(new BusinessLogicError("O arquivo deve ter no máximo 10 MB."));

        if (!IsImage(request.Content))
            return Result.Fail(new BusinessLogicError("O arquivo enviado deve ser uma imagem válida (jpg, png, gif, bmp, webp, tiff)."));

        var key = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
        var bucket = "products";

        var uri = await _storage.UploadAsync(bucket, key, request.Content, cancellationToken: cancellationToken);

        product.ChangeImage(ProductImage.Create(uri.AbsoluteUri));

        await _repository.UpdateAsync(product, cancellationToken);

        return Result.Ok(product);
    }

    private static bool IsImage(byte[] bytes)
    {
        if (bytes.Length < 4) return false;

        // JPEG: FF D8 FF
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return true;

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return true;

        // GIF: 'G','I','F','8','7'/'9','a'
        if (bytes.Length >= 6 && bytes[0] == (byte)'G' && bytes[1] == (byte)'I' && bytes[2] == (byte)'F')
            return true;

        // BMP: 'B','M'
        if (bytes[0] == 0x42 && bytes[1] == 0x4D)
            return true;

        // WebP: 'R','I','F','F'....'W','E','B','P'
        if (bytes.Length >= 12 && bytes[0] == (byte)'R' && bytes[1] == (byte)'I' && bytes[2] == (byte)'F' && bytes[3] == (byte)'F'
            && bytes[8] == (byte)'W' && bytes[9] == (byte)'E' && bytes[10] == (byte)'B' && bytes[11] == (byte)'P')
            return true;

        // TIFF little endian: 49 49 2A 00 or big endian: 4D 4D 00 2A
        if (bytes.Length >= 4 && ((bytes[0] == 0x49 && bytes[1] == 0x49 && bytes[2] == 0x2A && bytes[3] == 0x00) || (bytes[0] == 0x4D && bytes[1] == 0x4D && bytes[2] == 0x00 && bytes[3] == 0x2A)))
            return true;

        return false;
    }
}
