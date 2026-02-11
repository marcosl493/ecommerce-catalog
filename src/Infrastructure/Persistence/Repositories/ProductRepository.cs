using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(CatalogDbContext db) : IProductRepository
{
    private readonly CatalogDbContext _db = db;

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _db.Products.AddAsync(product, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
