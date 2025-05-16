using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace inventario_service.Domain.Entities
{
    public class Receta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public List<IngredienteReceta> Ingredientes { get; set; } = new List<IngredienteReceta>();
    }

    public class IngredienteReceta
    {
        public string IngredienteId { get; set; }
        public double Cantidad { get; set; }
    }
}