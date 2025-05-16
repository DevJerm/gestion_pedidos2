using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;

namespace inventario_service.Application.Services
{
    public class InventarioService: IInventarioService
    {
        private readonly IIngredienteRepository _ingredienteRepository;

        public InventarioService(IIngredienteRepository ingredienteRepository)
        {
            _ingredienteRepository = ingredienteRepository;
        }

        public async Task<IEnumerable<Ingrediente>> GetAllIngredientesAsync()
        {
            return await _ingredienteRepository.GetAllAsync();
        }

        public async Task<Ingrediente> GetIngredienteByIdAsync(string id)
        {
            return await _ingredienteRepository.GetByIdAsync(id);
        }

        public async Task CreateIngredienteAsync(Ingrediente ingrediente)
        {
            await _ingredienteRepository.CreateAsync(ingrediente);
        }

        public async Task UpdateIngredienteAsync(Ingrediente ingrediente)
        {
            await _ingredienteRepository.UpdateAsync(ingrediente);
        }

        public async Task DeleteIngredienteAsync(string id)
        {
            await _ingredienteRepository.DeleteAsync(id);
        }

        public async Task ActualizarInventario(List<Domain.Services.IngredienteRequerido> ingredientesRequeridos)
        {
            foreach (var ingredienteReq in ingredientesRequeridos)
            {
                var ingrediente = await _ingredienteRepository.GetByIdAsync(ingredienteReq.IngredienteId);
                if (ingrediente != null)
                {
                    ingrediente.Cantidad -= ingredienteReq.CantidadRequerida;
                    await _ingredienteRepository.UpdateAsync(ingrediente);
                }
            }
        }
    }
}