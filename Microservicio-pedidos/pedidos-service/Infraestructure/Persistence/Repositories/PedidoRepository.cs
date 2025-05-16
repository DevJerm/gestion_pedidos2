using MongoDB.Driver;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Domain.ValueObjects;
using pedidos_service.Infraestructure.Persistence.MongoDB;

namespace pedidos_service.Infraestructure.Persistence.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly IMongoCollection<Pedido> _pedidos;

        public PedidoRepository(IMongoDBContext context)
        {
            _pedidos = context.Pedidos;
        }

        public async Task<Pedido> GetByIdAsync(string id)
        {
            return await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _pedidos.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetByClienteIdAsync(string clienteId)
        {
            return await _pedidos.Find(p => p.ClienteId == clienteId).ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetByEstadoAsync(EstadoPedido estado)
        {
            return await _pedidos.Find(p => p.Estado == estado).ToListAsync();
        }

        public async Task AddAsync(Pedido pedido)
        {
            await _pedidos.InsertOneAsync(pedido);
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            await _pedidos.ReplaceOneAsync(p => p.Id == pedido.Id, pedido);
        }

        public async Task DeleteAsync(string id)
        {
            await _pedidos.DeleteOneAsync(p => p.Id == id);
        }
    }
}
