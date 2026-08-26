using Application.Interfaces;
using Domain.Entities;
using FluentResults;
using MediatR;
using System.Text.Json;

namespace Application.UseCases.CreateProduct;

public sealed class CreateProductHandler
    (
        IProductRepository repository,
        IPublisherEvent publisherEvent
    ) : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _repository = repository;
    private readonly IPublisherEvent _publisherEvent = publisherEvent;
    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        const string topic = "products";
        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Category);

        await _repository.AddAsync(product, cancellationToken);

        await _publisherEvent.ProduceEventAsync(topic, product.Id.ToString(), JsonSerializer.Serialize(product), cancellationToken);

        return new CreateProductResponse(product.Id);
    }
}
