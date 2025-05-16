using MongoDB.Driver;
using pedidos_service.Domain.Entities;
using pedidos_service.Domain.Repositories;
using pedidos_service.Infraestructure.Persistence.MongoDB;

namespace pedidos_service.Infraestructure.Persistence.Repositories
{
    // utilizamos el patron de repositorios, para acceso a db
    public class ClienteRepository : IClienteRepository
    {
        private readonly IMongoCollection<Cliente> _clientes;

        public ClienteRepository(IMongoDBContext context)
        {
            _clientes = context.Clientes;
        }

        public async Task<Cliente> GetByIdAsync(string id)
        {
            return await _clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Cliente> GetByEmailAsync(string email)
        {
            return await _clientes.Find(c => c.Email == email).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clientes.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _clientes.InsertOneAsync(cliente);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            await _clientes.ReplaceOneAsync(c => c.Id == cliente.Id, cliente);
        }

        public async Task DeleteAsync(string id)
        {
            await _clientes.DeleteOneAsync(c => c.Id == id);
        }
    }
}
