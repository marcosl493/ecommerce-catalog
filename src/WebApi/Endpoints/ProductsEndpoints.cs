using Application.UseCases.CreateProduct;
using Application.UseCases.DeleteProduct;
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

        return endpoints;
    }
}
