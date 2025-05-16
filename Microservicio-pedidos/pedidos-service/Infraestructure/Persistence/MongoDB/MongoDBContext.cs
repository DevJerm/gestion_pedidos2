using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pedidos_service.Domain.Entities;


namespace pedidos_service.Infraestructure.Persistence.MongoDB
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Pedido> Pedidos => _database.GetCollection<Pedido>("Pedidos");
        public IMongoCollection<Cliente> Clientes => _database.GetCollection<Cliente>("Clientes");
    }

    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}