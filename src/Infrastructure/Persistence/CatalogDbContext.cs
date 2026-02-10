using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class CatalogDbContext
    (
        DbContextOptions<CatalogDbContext> options
    ) : DbContext(options)
{
}
