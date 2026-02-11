using FluentResults;
using MediatR;
using Domain.Entities;

namespace Application.UseCases.UploadProductImage;

public sealed record UploadProductImageCommand(Guid Id, byte[] Content, string FileName) : IRequest<Result<Product>>;
