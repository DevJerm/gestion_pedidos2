namespace pedidos_service.Application.DTOs
{
    public class ClienteDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DireccionEntregaDTO DireccionPredeterminada { get; set; }
    }
    public class CrearClienteDTO
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DireccionEntregaDTO DireccionEntrega { get; set; }
    }
}
