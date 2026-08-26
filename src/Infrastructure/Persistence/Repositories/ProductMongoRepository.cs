using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public sealed class ProductMongoRepository
    (
        IMongoDatabase mongoDatabase
    ) : IProductRepository
{
    private const string CollectionName = "Products";

    private readonly IMongoCollection<Product> _collection = mongoDatabase.GetCollection<Product>(CollectionName);

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        return _collection.InsertOneAsync(product, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        return _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<Product>> QueryAsync(decimal? minPrice = null, decimal? maxPrice = null, bool? active = null, ProductCategory? category = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Empty;

        if (minPrice.HasValue)
            filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Gte(p => p.Price, minPrice.Value));

        if (maxPrice.HasValue)
            filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Lte(p => p.Price, maxPrice.Value));

        if (active.HasValue)
            filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Active, active.Value));

        if (category.HasValue && category.Value != ProductCategory.Undefined)
            filter = Builders<Product>.Filter.And(filter, Builders<Product>.Filter.Eq(p => p.Category, category.Value));

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var items = await _collection
            .Find(filter)
            .Sort(Builders<Product>.Sort.Ascending(p => p.Id))
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        return _collection.ReplaceOneAsync(filter, product, new ReplaceOptions { IsUpsert = false }, cancellationToken);
    }
}
