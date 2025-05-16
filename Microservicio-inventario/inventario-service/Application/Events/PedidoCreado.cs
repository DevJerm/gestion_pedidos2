namespace inventario_service.Application.Events
{
    public class PedidoCreadoEvent
    {
        public string PedidoId { get; set; }
        public string ClienteId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<ItemPedidoEvent> Items { get; set; }

        public class ItemPedidoEvent
        {
            public string ProductoId { get; set; }
            public int Cantidad { get; set; }
        }
    }

}