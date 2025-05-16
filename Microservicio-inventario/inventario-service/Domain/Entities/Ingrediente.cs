using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace inventario_service.Domain.Entities
{
    public class Ingrediente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public double Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public bool Disponible { get; set; }
    }
}