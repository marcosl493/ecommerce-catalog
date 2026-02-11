using Application.Interfaces;
using Application.Reasons;
using Domain.Entities;
using FluentResults;
using MediatR;

namespace Application.UseCases.EditProduct;

public sealed class EditProductHandler(
    IProductRepository repository
) : IRequestHandler<EditProductCommand, Result<Product>>
{
    private readonly IProductRepository _repository = repository;

    public async Task<Result<Product>> Handle(EditProductCommand request, CancellationToken cancellationToken)
    {

        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return Result.Fail(new ResourceNotFoundError("Produto"));
        var changed = false;

        if (request.Name is not null && request.Name != product.Name)
        {
            product.ChangeName(request.Name);
            changed = true;
        }

        if (request.Description is not null && request.Description != product.Description)
        {
            product.ChangeDescription(request.Description);
            changed = true;
        }

        if (request.Price.HasValue && request.Price.Value != product.Price)
        {
            product.ChangePrice(request.Price.Value);
            changed = true;
        }

        if (request.Category.HasValue && request.Category.Value != product.Category)
        {
            product.ChangeCategory(request.Category.Value);
            changed = true;
        }

        if (request.Active.HasValue)
        {
            if (request.Active.Value != product.Active)
            {
                if (request.Active.Value)
                    product.Activate();
                else
                    product.Deactivate();

                changed = true;
            }
        }

        if (!changed)
            return Result.Ok();

        await _repository.UpdateAsync(product, cancellationToken);

        return Result.Ok(product);

    }
}
