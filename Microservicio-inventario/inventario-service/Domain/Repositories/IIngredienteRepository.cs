using inventario_service.Domain.Entities;

namespace inventario_service.Domain.Repositories
{
    public interface IIngredienteRepository
    {
        Task<IEnumerable<Ingrediente>> GetAllAsync();
        Task<Ingrediente> GetByIdAsync(string id);
        Task<IEnumerable<Ingrediente>> GetByIdsAsync(IEnumerable<string> ids);
        Task<List<Ingrediente>> GetByNombresAsync(List<string> nombres);
        Task CreateAsync(Ingrediente ingrediente);
        Task UpdateAsync(Ingrediente ingrediente);
        Task DeleteAsync(string id);
    }
}