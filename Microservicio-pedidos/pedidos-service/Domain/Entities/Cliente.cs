using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Domain.Entities
{
    public class Cliente
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public DireccionEntrega DireccionPredeterminada { get; private set; }

        public Cliente(string nombre, string email, string telefono, DireccionEntrega direccionPredeterminada)
        {
            Id = Guid.NewGuid().ToString();
            Nombre = nombre;
            Email = email;
            Telefono = telefono;
            DireccionPredeterminada = direccionPredeterminada;
        }
    }
}
