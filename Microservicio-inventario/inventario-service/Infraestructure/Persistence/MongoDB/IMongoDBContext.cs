using inventario_service.Domain.Entities;
using MongoDB.Driver;

namespace inventario_service.Infraestructure.Persistence.MongoDB
{
    public interface IMongoDBContext
    {
        IMongoCollection<Receta> Recetas { get; }
        IMongoCollection<Ingrediente> Ingredientes { get; }
    }

}
