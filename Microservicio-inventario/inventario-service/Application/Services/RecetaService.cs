using inventario_service.Domain.Entities;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;

namespace inventario_service.Application.Services
{
    public class RecetaService: IRecetaService
    {
        private readonly IRecetaRepository _recetaRepository;

        public RecetaService(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<IEnumerable<Receta>> GetAllRecetasAsync()
        {
            return await _recetaRepository.GetAllAsync();
        }

        public async Task<Receta> GetRecetaByIdAsync(string id)
        {
            return await _recetaRepository.GetByIdAsync(id);
        }

        public async Task<Receta> GetRecetaByNombreAsync(string nombre)
        {
            return await _recetaRepository.GetByNombreAsync(nombre);
        }

        public async Task CreateRecetaAsync(Receta receta)
        {
            await _recetaRepository.CreateAsync(receta);
        }

        public async Task UpdateRecetaAsync(Receta receta)
        {
            await _recetaRepository.UpdateAsync(receta);
        }

        public async Task DeleteRecetaAsync(string id)
        {
            await _recetaRepository.DeleteAsync(id);
        }
    }
}