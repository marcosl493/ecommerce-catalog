using Application.Common;
using Application.UseCases.CreateProduct;
using Application.UseCases.DeleteProduct;
using Application.UseCases.GetProduct;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Endpoints;

public static class ProductsEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var endpoints = endpoint
           .MapGroup("api/products")
           .WithTags("Product")
           .ProducesProblem(StatusCodes.Status500InternalServerError);

        endpoints.MapPost(string.Empty, async (
            [FromBody] CreateProductCommand request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await sender.Send(request, cancellationToken);

            return result.Serialize();
        }).Produces<CreateProductResponse>(StatusCodes.Status200OK)
          .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        endpoints.MapDelete("/{id:guid}", async (
            [FromRoute] Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
            return result.Serialize();
        }).Produces(StatusCodes.Status204NoContent)
          .ProducesProblem(StatusCodes.Status404NotFound);

        endpoints.MapGet("", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            [FromQuery] Guid? id,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? active,
            [FromQuery] ProductCategory? category,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        ) =>
        {
            var query = new GetProductQuery
            {
                Id = id,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Active = active,
                Category = category,
                Page = page,
                PageSize = pageSize
            };

            var result = await sender.Send(query, cancellationToken);
            return result.Serialize();
        }).Produces<PagedResult<Product>>(StatusCodes.Status200OK)
          .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
