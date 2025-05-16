namespace inventario_service.Application.DTOs
{
    public class RecetaDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public List<IngredienteRecetaDTO> Ingredientes { get; set; } = new List<IngredienteRecetaDTO>();
    }

    public class IngredienteRecetaDTO
    {
        public string IngredienteId { get; set; }
        public double Cantidad { get; set; }
    }
}