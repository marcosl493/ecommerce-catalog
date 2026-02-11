using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
}
