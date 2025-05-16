using inventario_service.Domain.Entities;

namespace inventario_service.Domain.Services
{
    public interface IInventarioService
    {
        Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync();
        Task<Ingrediente> GetIngredienteByIdAsync(string id);
        Task CreateIngredienteAsync(Ingrediente ingrediente);
        Task UpdateIngredienteAsync(Ingrediente ingrediente);
        Task DeleteIngredienteAsync(string id);
        Task ActualizarInventario(List<IngredienteRequerido> ingredientesRequeridos);
    }
}
