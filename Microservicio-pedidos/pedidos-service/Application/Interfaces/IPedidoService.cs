using pedidos_service.Application.DTOs;

namespace pedidos_service.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoDTO> CrearPedidoAsync(CrearPedidoDTO crearPedidoDTO);
        Task<PedidoDTO> GetPedidoByIdAsync(string pedidoId);
        Task<IEnumerable<PedidoDTO>> GetPedidosByClienteIdAsync(string clienteId);
        Task<IEnumerable<PedidoDTO>> GetPedidosByEstadoAsync(string estado);
        Task<PedidoDTO> ActualizarEstadoPedidoAsync(string pedidoId, string nuevoEstado);
    }
}
