using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Services;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Application.Services
{
    public class CreacionPedidoService : ICreacionPedidoService
    {
        public async Task<Pedido> CrearPedidoAsync(
            string clienteId,
            DireccionEntrega direccionEntrega,
            List<(string productoId, int cantidad, decimal precioUnitario)> items)
        {
           
            if (items == null || !items.Any())
                throw new ArgumentException("Un pedido debe contener al menos un item");

            var pedido = new Pedido(clienteId, direccionEntrega);

            foreach (var (productoId, cantidad, precioUnitario) in items)
            {
                if (cantidad <= 0)
                    throw new ArgumentException($"La cantidad debe ser mayor que cero para el producto {productoId}");

                if (precioUnitario <= 0)
                    throw new ArgumentException($"El precio debe ser mayor que cero para el producto {productoId}");

                pedido.AgregarItem(productoId, cantidad, precioUnitario);
            }

            return pedido;
        }
    }
}
