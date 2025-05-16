using MongoDB.Driver;
using pedidos_service.Domain.Entities;

namespace pedidos_service.Infraestructure.Persistence.MongoDB
{
    public interface IMongoDBContext
    {
        IMongoCollection<Pedido> Pedidos { get; }
        IMongoCollection<Cliente> Clientes { get; }
    }

}
