using Application.Common;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> QueryAsync(
           decimal? minPrice = null,
           decimal? maxPrice = null,
           bool? active = null,
           ProductCategory? category = null,
           int page = 1,
           int pageSize = 10,
           CancellationToken cancellationToken = default);
}
