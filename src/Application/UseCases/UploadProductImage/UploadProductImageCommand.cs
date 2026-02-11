using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.UploadProductImage;

public sealed record UploadProductImageCommand(Guid Id, byte[] Content, string FileName) : IRequest<Result<Product>>;
