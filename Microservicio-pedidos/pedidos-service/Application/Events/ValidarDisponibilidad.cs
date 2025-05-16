namespace inventario_service.Application.Events
{
    public class DisponibilidadValidadaEvent
    {
        public string PedidoId { get; set; }
        public bool Disponible { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}