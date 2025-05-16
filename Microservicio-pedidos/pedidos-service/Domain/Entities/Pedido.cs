using pedidos_service.Domain;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Domain.Entities
{
    public class Pedido
    {
        public string Id { get; private set; }
        public string ClienteId { get; private set; }
        public List<ItemPedido> Items { get; private set; }
        public EstadoPedido Estado { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DireccionEntrega DireccionEntrega { get; private set; }
        public TotalPedido Total { get; private set; }

        public Pedido(string clienteId, DireccionEntrega direccionEntrega)
        {
            Id = Guid.NewGuid().ToString();
            ClienteId = clienteId;
            Items = new List<ItemPedido>();
            Estado = EstadoPedido.CREADO;
            FechaCreacion = DateTime.UtcNow;
            DireccionEntrega = direccionEntrega;
            Total = new TotalPedido(0);
        }

        public void AgregarItem(string productoId, int cantidad, decimal precioUnitario)
        {
            var item = new ItemPedido(productoId, cantidad, precioUnitario);
            Items.Add(item);
            RecalcularTotal();
        }

        public void ConfirmarPedido()
        {
            if (Estado != EstadoPedido.CREADO)
                throw new InvalidOperationException("Solo se pueden confirmar pedidos en estado CREADO");

            Estado = EstadoPedido.CONFIRMADO;
        }
        public void MarcarSinStock()
        {
            if (Estado != EstadoPedido.CREADO)
                throw new InvalidOperationException("Solo se pueden cancelar pedidos en estado CREADO");

            Estado = EstadoPedido.SIN_STOCK;
        }

        public void MarcarEnPreparacion()
        {
            if (Estado != EstadoPedido.CONFIRMADO)
                throw new InvalidOperationException("Solo se pueden preparar pedidos CONFIRMADOS");

            Estado = EstadoPedido.EN_PREPARACION;
        }

        public void MarcarListo()
        {
            if (Estado != EstadoPedido.EN_PREPARACION)
                throw new InvalidOperationException("Solo pueden estar listos los pedidos EN_PREPARACION");

            Estado = EstadoPedido.LISTO;
        }

        public void MarcarEntregado()
        {
            if (Estado != EstadoPedido.LISTO)
                throw new InvalidOperationException("Solo se pueden entregar pedidos LISTOS");

            Estado = EstadoPedido.ENTREGADO;
        }

        private void RecalcularTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Subtotal;
            }
            Total = new TotalPedido(total);
        }
    }
}
