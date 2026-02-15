using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("MongoDB")
            ?? "mongodb://localhost:27017";
        var databaseName = config["DatabaseName"] ?? "LibraryManagement";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
        => _database.GetCollection<T>(name);
}