using pedidos_service.Domain.Entities;
using pedidos_service.Domain.ValueObjects;

namespace pedidos_service.Domain.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> GetByIdAsync(string id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<IEnumerable<Pedido>> GetByClienteIdAsync(string clienteId);
        Task<IEnumerable<Pedido>> GetByEstadoAsync(EstadoPedido estado);
        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task DeleteAsync(string id);
    }
}
