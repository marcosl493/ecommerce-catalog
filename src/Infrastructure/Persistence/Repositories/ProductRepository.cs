using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(CatalogDbContext db) : IProductRepository
{
    private readonly CatalogDbContext _db = db;

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _db.Products.AddAsync(product, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Product>> QueryAsync(decimal? minPrice = null, decimal? maxPrice = null, bool? active = null, ProductCategory? category = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _db.Products.AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (active.HasValue)
            query = query.Where(p => p.Active == active.Value);

        if (category.HasValue && category.Value != ProductCategory.Undefined)
            query = query.Where(p => p.Category == category.Value);

        var total = await query.CountAsync(cancellationToken);

        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
