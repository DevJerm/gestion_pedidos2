namespace pedidos_service.Application.DTOs
{

    public class CrearPedidoDTO
    {
        public string ClienteId { get; set; }
        public DireccionEntregaDTO DireccionEntrega { get; set; }
        public List<ItemPedidoDTO> Items { get; set; }
    }

    public class DireccionEntregaDTO
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }
        public string Referencia { get; set; }
    }

    public class ItemPedidoDTO
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
