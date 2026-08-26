using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence;

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    public MongoDbContext(IOptions<Options> options)
    {
        _client = new MongoClient(options.Value.ConnectionString);
        _database = _client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoDatabase GetMongoDatabase() => _database;
    public sealed class Options
    {
        public const string SectionName = "MongoDb";
        [Required]
        public string ConnectionString { get; set; } = null!;
        [Required]
        public string DatabaseName { get; set; } = null!;
    }
}
