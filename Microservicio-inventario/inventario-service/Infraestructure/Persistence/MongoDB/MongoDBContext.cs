using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace inventario_service.Infraestructure.Persistence.MongoDB
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _database;
        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }
        public IMongoCollection<Domain.Entities.Ingrediente> Ingredientes => _database.GetCollection<Domain.Entities.Ingrediente>("Ingredientes");
        public IMongoCollection<Domain.Entities.Receta> Recetas => _database.GetCollection<Domain.Entities.Receta>("Recetas");
    }
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}