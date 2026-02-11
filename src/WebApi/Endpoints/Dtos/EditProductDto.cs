using Domain.Entities;

namespace WebApi.Endpoints.Dtos;

public sealed record EditProductDto
    (
        string? Name,
        string? Description,
        decimal? Price,
        bool? Active,
        ProductCategory? Category
    );
