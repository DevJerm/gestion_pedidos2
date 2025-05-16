using inventario_service.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace inventario_service.Domain.Services
{
    public interface IValidacionDisponibilidadService
    {
        Task<bool> ValidarDisponibilidadIngredientes(List<IngredienteRequerido> ingredientesRequeridos);
        Task<bool> ValidarDisponibilidadIngredientesPorNombre(List<IngredienteRequeridoPorNombre> ingredientesRequeridos);
    }

    public class IngredienteRequerido
    {
        public string IngredienteId { get; set; }
        public double CantidadRequerida { get; set; }
    }
}