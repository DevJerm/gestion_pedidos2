using pedidos_service.Domain.Entities;

namespace pedidos_service.Domain.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente> GetByIdAsync(string id);
        Task<Cliente> GetByEmailAsync(string email);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(string id);
    }
}
