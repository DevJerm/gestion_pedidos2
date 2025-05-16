using System.Collections.Generic;

namespace inventario_service.Application.DTOs
{
    public class ValidarDisponibilidadRequestDTO
    {
        public List<IngredienteRequeridoDTO> Ingredientes { get; set; } = new List<IngredienteRequeridoDTO>();
    }

    public class IngredienteRequeridoDTO
    {
        public string IngredienteId { get; set; }
        public double CantidadRequerida { get; set; }
    }
}