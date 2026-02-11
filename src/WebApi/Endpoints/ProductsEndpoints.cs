using Application.Common;
using Application.UseCases.CreateProduct;
using Application.UseCases.DeleteProduct;
using Application.UseCases.EditProduct;
using Application.UseCases.GetProduct;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints.Dtos;

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
        })
        .WithDescription("Cria um novo produto")
        .Produces<CreateProductResponse>(StatusCodes.Status200OK)
          .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        endpoints.MapDelete("/{id:guid}", async (
            [FromRoute] Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
            return result.Serialize();
        })
        .WithDescription("Remove o produto especificado pelo id")
        .Produces(StatusCodes.Status204NoContent)
          .ProducesProblem(StatusCodes.Status404NotFound);

        endpoints.MapPut("/{id:guid}", async (
            [FromRoute] Guid id,
            [FromBody] EditProductDto request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken
        ) =>
        {
            var command = new EditProductCommand(
                Id: id,
                Name: request.Name,
                Description: request.Description,
                Price: request.Price,
                Active: request.Active,
                Category: request.Category
            );
            var result = await sender.Send(command, cancellationToken);
            return result.Serialize();
        })
        .WithDescription("Edita o produto especificado pelo id")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent)
          .ProducesProblem(StatusCodes.Status404NotFound)
          .ProducesValidationProblem(StatusCodes.Status400BadRequest);

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
        })
        .WithDescription("Retorna lista de produtos ou um produto específico quando 'id' for informado. Suporta filtros por preço, categoria, status e paginação")
        .Produces<PagedResult<Product>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
