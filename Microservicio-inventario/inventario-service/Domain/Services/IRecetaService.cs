using inventario_service.Domain.Entities;

namespace inventario_service.Domain.Services
{
    public interface IRecetaService
    {
        Task<IEnumerable<Receta>> GetAllRecetasAsync();
        Task<Receta> GetRecetaByIdAsync(string id);
        Task<Receta> GetRecetaByNombreAsync(string nombre);
        Task CreateRecetaAsync(Receta receta);
        Task UpdateRecetaAsync(Receta receta);
        Task DeleteRecetaAsync(string id);
    }
}
