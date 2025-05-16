using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Domain.Services
{
    public interface ICreacionPedidoService
    {
        Task<Pedido> CrearPedidoAsync(string clienteId, DireccionEntrega direccionEntrega, List<(string productoId, int cantidad, decimal precioUnitario)> items);
    }
}
