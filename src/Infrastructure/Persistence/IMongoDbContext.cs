using MongoDB.Driver;

namespace Infrastructure.Persistence;

public interface IMongoDbContext
{
    IMongoDatabase GetMongoDatabase();
}
