using inventario_service.Application.DTOs;
using inventario_service.Domain.Repositories;
using inventario_service.Domain.Services;

namespace inventario_service.Application.Services
{
    public class ValidacionDisponibilidadService : IValidacionDisponibilidadService
    {
        private readonly IIngredienteRepository _ingredienteRepository;

        public ValidacionDisponibilidadService(IIngredienteRepository ingredienteRepository)
        {
            _ingredienteRepository = ingredienteRepository;
        }

        public async Task<bool> ValidarDisponibilidadIngredientes(List<IngredienteRequerido> ingredientesRequeridos)
        {
            var ingredienteIds = ingredientesRequeridos.Select(i => i.IngredienteId).ToList();
            var ingredientes = await _ingredienteRepository.GetByIdsAsync(ingredienteIds);
            var ingredientesDict = ingredientes.ToDictionary(i => i.Id);

            foreach (var ingredienteRequerido in ingredientesRequeridos)
            {
                if (!ingredientesDict.TryGetValue(ingredienteRequerido.IngredienteId, out var ingrediente))
                {
                    return false; // No existe el ingrediente
                }

                if (!ingrediente.Disponible)
                {
                    return false; // Ingrediente no disponible
                }

                if (ingrediente.Cantidad < ingredienteRequerido.CantidadRequerida)
                {
                    return false; // No hay suficiente cantidad
                }
            }

            return true;
        }
        public async Task<bool> ValidarDisponibilidadIngredientesPorNombre(List<IngredienteRequeridoPorNombre> ingredientesRequeridos)
        {
            var nombresIngredientes = ingredientesRequeridos.Select(i => i.Nombre).ToList();
            var ingredientes = await _ingredienteRepository.GetByNombresAsync(nombresIngredientes);
            var ingredientesDict = ingredientes.ToDictionary(i => i.Nombre.ToLower());

            foreach (var ingredienteRequerido in ingredientesRequeridos)
            {
                if (!ingredientesDict.TryGetValue(ingredienteRequerido.Nombre.ToLower(), out var ingrediente))
                {
                    return false; // No existe el ingrediente
                }

                if (!ingrediente.Disponible)
                {
                    return false; // Ingrediente no disponible
                }

                if (ingrediente.Cantidad < ingredienteRequerido.CantidadRequerida)
                {
                    return false; // No hay suficiente cantidad
                }
            }

            return true;
        }
    }
}