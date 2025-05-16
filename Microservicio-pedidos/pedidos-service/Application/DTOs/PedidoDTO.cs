namespace pedidos_service.Application.DTOs
{
    public class PedidoDTO
    {
        public string Id { get; set; }
        public string ClienteId { get; set; }
        public List<ItemPedidoDTO> Items { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DireccionEntregaDTO DireccionEntrega { get; set; }
        public decimal Total { get; set; }
    }
}
