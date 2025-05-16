using inventario_service.Domain.Entities;

namespace inventario_service.Domain.Repositories
{
    public interface IRecetaRepository
    {
        Task<IEnumerable<Receta>> GetAllAsync();
        Task<Receta> GetByIdAsync(string id);
        Task<Receta> GetByNombreAsync(string nombre);
        Task CreateAsync(Receta receta);
        Task UpdateAsync(Receta receta);
        Task DeleteAsync(string id);
    }
}