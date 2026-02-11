using FluentResults;
using MediatR;

namespace Application.UseCases.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id)
    : IRequest<Result>;